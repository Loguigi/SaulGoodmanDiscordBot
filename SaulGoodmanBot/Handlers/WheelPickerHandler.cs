using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class WheelPickerHandler {
    public static async Task HandleAdd(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id != IDHelper.WheelPicker.Add) {
            await Task.CompletedTask;
            return;
        }

        var interactivity = s.GetInteractivity();
        var wheel = new WheelPickers(e.Guild).Wheels[e.Values.First()];

        // display starting message
        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Adding to {wheel.Name}...")
            .WithDescription("Type an option to add or `stop` to stop adding\nTo add multiple options, separate by a new line")
            .AddField("Last Added", "---", false)
            .AddField("Status", "Still listening", true)
            .AddField("Total Options", wheel.GetAllOptions().Count.ToString(), true);
        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
        var response = await interactivity.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));

        while (!response.Result.Content.ToLower().Contains("stop") && !response.TimedOut) {
            if (response.Result.Content.Contains('\n')) {
                foreach (var o in response.Result.Content.Split('\n')) {
                    wheel.AddOption(o);
                    embed.Fields.Where(x => x.Name == "Last Added").First().Value = o;
                }
            } else {
                var option = response.Result.Content;
                wheel.AddOption(option);
                embed.Fields.Where(x => x.Name == "Last Added").First().Value = option;
            }
            
            embed.Fields.Where(x => x.Name == "Total Options").First().Value = wheel.GetAllOptions().Count.ToString();
            await e.Channel.DeleteMessageAsync(response.Result);
            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            response = await interactivity.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));
        }

        embed.Fields.Where(x => x.Name == "Status").First().Value = "Finished";
        embed.WithColor(DiscordColor.Green);
        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

        s.ComponentInteractionCreated -= HandleAdd;
    }

    public static async Task HandleSpin(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.Spin)) {
            await Task.CompletedTask;
            return;
        }

        var first_spin = e.Id == IDHelper.WheelPicker.Spin;

        var wheel = new WheelPickers(e.Guild).Wheels[first_spin ? e.Values.First() : e.Id.Split('\\')[WHEEL_NAME_INDEX]];
        int spin_count = first_spin ? 1 : int.Parse(e.Id.Split('\\')[SPIN_COUNT_INDEX]);
        var last_option_spun = first_spin ? "---" : e.Id.Split('\\')[LAST_OPTION_SPUN_INDEX];
        var temp_delete = !first_spin && bool.Parse(e.Id.Split('\\')[REMOVE_INDEX]);

        if (temp_delete) {
            wheel.TemporarilyRemoveOption(last_option_spun);    
        }

        var result = wheel.Spin();

        var buttons = new List<DiscordButtonComponent>() {
            new(ButtonStyle.Success, $"{IDHelper.WheelPicker.Spin}\\{wheel.Name}\\{spin_count + 1}\\{result}\\false", "Spin again", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":repeat:", false))),
            new(ButtonStyle.Danger, $"{IDHelper.WheelPicker.Spin}\\{wheel.Name}\\{spin_count + 1}\\{result}\\true", "Remove and spin again", wheel.Options.Count == 1, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":repeat:", false)))
        };

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"{DiscordEmoji.FromName(s, ":cyclone:", false)} {wheel.Name} {DiscordEmoji.FromName(s, ":cyclone:", false)}")
            .WithDescription($"# {result}")
            .WithThumbnail(wheel.Image ?? "")
            .AddField("Spin Count", spin_count.ToString(), true)
            .AddField("Total Options", $"{wheel.Options.Count} / {wheel.Options.Count + wheel.RemovedOptions.Count}", true)
            .AddField("Last Spun", temp_delete ? $"{DiscordEmoji.FromName(s, ":x:", false)} {last_option_spun} {DiscordEmoji.FromName(s, ":x:", false)}" : last_option_spun, false)
            .WithColor(DiscordColor.Cyan)
            .WithFooter($"Spun by {e.User.GlobalName}", e.User.AvatarUrl);

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(buttons)));
    }

    public static async Task HandleDeleteWheelSelection(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.DeleteWheel)) {
            await Task.CompletedTask;
            return;
        }

        var first_interaction = e.Id == IDHelper.WheelPicker.DeleteWheel;
        var serverWheels = new WheelPickers(e.Guild);
        var wheel = serverWheels.Wheels[first_interaction ? e.Values.First() : e.Id.Split('\\')[WHEEL_NAME_INDEX]];
        var interactivity = new InteractivityHelper<string>(s, wheel.GetAllOptions(), $"{IDHelper.WheelPicker.DeleteWheel}\\{wheel.Name}", first_interaction ? "1" : e.Id.Split('\\')[PAGE_INDEX]);

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Deleting from {wheel.Name}")
            .WithDescription($"{wheel.GetAllOptions().Count} total options\n\n")
            .WithFooter(interactivity.PageStatus())
            .WithColor(DiscordColor.DarkRed);

        var options = new List<DiscordSelectComponentOption>();
        foreach (var option in interactivity.GetPage()) {
            options.Add(new DiscordSelectComponentOption(option, option, ""));
            embed.Description += $"`{wheel.Options.IndexOf(option) + 1}`. {option}\n";
        }
        options.Add(new DiscordSelectComponentOption($"Delete wheel \"{wheel.Name}\"", DELETE_ENTIRE_WHEEL, "Warning: this is irreversible", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":fire:", false))));
        options.Add(new DiscordSelectComponentOption("Cancel", CANCEL_DELETE, "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":back:", false))));
        var dropdown = new DiscordSelectComponent($"{IDHelper.WheelPicker.DeleteOption}\\{wheel.Name}", "Select an option", options);

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(
            interactivity.AddPageButtons().AddEmbed(embed).AddComponents(dropdown)));
        
        s.ComponentInteractionCreated -= HandleDeleteOptionSelection;
        s.ComponentInteractionCreated += HandleDeleteOptionSelection;
    }

    public static async Task HandleDeleteOptionSelection(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.DeleteOption)) {
            await Task.CompletedTask;
            return;
        }

        var option = e.Values.First();
        var serverWheels = new WheelPickers(e.Guild);
        var wheel = serverWheels.Wheels[e.Id.Split('\\')[WHEEL_NAME_INDEX]];
        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Deleting from {wheel.Name}")
            .WithColor(DiscordColor.DarkRed);

        switch (option) {
            case CANCEL_DELETE:
                embed.AddField("Result", "Cancelled", true).AddField("Options Remaining", wheel.GetAllOptions().Count.ToString(), true);
                break;

            case DELETE_ENTIRE_WHEEL:
                serverWheels.DeleteWheel(wheel);
                embed.AddField("Result", $"Deleted saved wheel", true).AddField("Options Remaining", "0", true);
                break;

            default:
                wheel.DeleteOption(option);
                embed.AddField("Result", $"Deleted {option}", true).AddField("Options Remaining", wheel.GetAllOptions().Count.ToString(), true);
                break;
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

        s.ComponentInteractionCreated -= HandleDeleteWheelSelection;
        s.ComponentInteractionCreated -= HandleDeleteOptionSelection;
    }

    public static async Task HandleList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.List)) {
            await Task.CompletedTask;
            return;
        }

        string wheelName = e.Id == IDHelper.WheelPicker.List ? e.Values.First() : e.Id.Split('\\')[WHEEL_NAME_INDEX];
        var wheel = new WheelPickers(e.Guild).Wheels[wheelName];    
        var interactivity = new InteractivityHelper<string>(s, wheel.GetAllOptions(), $"{IDHelper.WheelPicker.List}\\{wheelName}", e.Id == IDHelper.WheelPicker.List ? "1" : e.Id.Split('\\')[PAGE_INDEX], "There are no options in this wheel");

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
            .WithTitle(wheel.Name)
            .WithThumbnail(wheel.Image ?? "")
            .WithDescription(interactivity.IsEmpty())
            .WithColor(DiscordColor.MidnightBlue)
            .WithFooter(interactivity.PageStatus());

        // add wheel options to description
        foreach (var o in interactivity.GetPage()) {
            embed.Description += wheel.RemovedOptions.Contains(o) ? $"`{wheel.GetAllOptions().IndexOf(o) + 1}`. ~~{o}~~\n" : $"`{wheel.GetAllOptions().IndexOf(o) + 1}`. {o}\n";
        }

        var refresh_button = new DiscordButtonComponent(ButtonStyle.Primary, $"{IDHelper.WheelPicker.List}\\{wheelName}\\{interactivity.Page}", "Refresh", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":repeat:", false)));

        // display response
        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(
            interactivity.AddPageButtons().AddEmbed(embed).AddComponents(refresh_button)
        ));
    }

    public static async Task HandleReload(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.ReloadWheel)) {
            await Task.CompletedTask;
            return;
        }

        var wheel = new WheelPickers(e.Guild).Wheels[e.Values.First()];
        wheel.Reload();

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Reloaded {wheel.Name}")
            .WithDescription($"Restored {wheel.RemovedOptions.Count} options")
            .WithColor(DiscordColor.Teal);

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

        s.ComponentInteractionCreated -= HandleReload;
    }

    private const int WHEEL_NAME_INDEX = 1;
    private const int PAGE_INDEX = 2;
    private const int SPIN_COUNT_INDEX = 2;
    private const int LAST_OPTION_SPUN_INDEX = 3;
    private const int REMOVE_INDEX = 4;
    private const string DELETE_ENTIRE_WHEEL = "*";
    private const string CANCEL_DELETE = "?";
}
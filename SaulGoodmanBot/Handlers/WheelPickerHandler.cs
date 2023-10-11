using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using System.ComponentModel.DataAnnotations;

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
            .WithDescription("Type an option to add or `stop` to stop adding")
            .AddField("Last Added", wheel.Options.Last(), false)
            .AddField("Status", "Still listening", true)
            .AddField("Total Options", (wheel.Options.Count + wheel.RemovedOptions.Count).ToString());
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

            await e.Channel.DeleteMessageAsync(response.Result);
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            response = await interactivity.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));
        }

        embed.Fields.Where(x => x.Name == "Status").First().Value = "Finished";
        embed.WithColor(DiscordColor.Green);
        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

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
            .WithDescription($"# {wheel.Spin()}")
            .WithThumbnail(wheel.Image ?? "")
            .AddField("Spin Count", spin_count.ToString(), true)
            .AddField("Total Options", $"{wheel.Options.Count} / {wheel.Options.Count + wheel.RemovedOptions.Count}", true)
            .AddField("Last Spun", (temp_delete) ? $"{DiscordEmoji.FromName(s, ":x:", false)} {last_option_spun} {DiscordEmoji.FromName(s, ":x:", false)}" : last_option_spun, false)
            .WithColor(DiscordColor.Cyan)
            .WithFooter($"Spun by {e.User.GlobalName}", e.User.AvatarUrl);

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(buttons)));
    }

    public static async Task HandleDelete(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.Delete)) {
            await Task.CompletedTask;
            return;
        }

        var first_interaction = e.Id == IDHelper.WheelPicker.Delete;

        var serverWheels = new WheelPickers(e.Guild);
        var wheel = serverWheels.Wheels[first_interaction ? e.Values.First() : e.Id.Split('\\')[WHEEL_NAME_INDEX]];
        int page = first_interaction ? 1 : int.Parse(e.Id.Split('\\')[PAGE_INDEX]);
        var interactivity = new InteractivityHelper<string>(s, wheel.GetAllOptions(), $"{IDHelper.WheelPicker.Delete}\\{wheel.Name}", page);

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Deleting from {wheel.Name}");



        s.ComponentInteractionCreated -= HandleDelete;
    }

    public static async Task HandleList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.WheelPicker.List)) {
            await Task.CompletedTask;
            return;
        }

        string wheelName = e.Id == IDHelper.WheelPicker.List ? e.Values.First() : e.Id.Split('\\')[WHEEL_NAME_INDEX];
        int page = e.Id == IDHelper.WheelPicker.List ? 1 : int.Parse(e.Id.Split('\\')[PAGE_INDEX]);
        var serverWheels = new WheelPickers(e.Guild);
        var interactivity = new InteractivityHelper<string>(s, serverWheels.Wheels[wheelName].Options, $"{IDHelper.WheelPicker.List}\\{wheelName}", page, "There are no options in this wheel");

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
            .WithTitle(serverWheels.Wheels[wheelName].Name)
            .WithThumbnail(serverWheels.Wheels[wheelName].Image ?? "")
            .WithDescription(interactivity.IsEmpty())
            .WithColor(DiscordColor.MidnightBlue);

        // add wheel options to description
        foreach (var o in interactivity.GetPage()) {
            embed.Description += $"`{interactivity.GetPage().IndexOf(o) + 1}`. {o}\n";
        }

        // display response
        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(
            interactivity.AddPageButtons().AddEmbed(embed)
        ));
    }

    private static readonly int WHEEL_NAME_INDEX = 1;
    private static readonly int PAGE_INDEX = 2;
    private static readonly int SPIN_COUNT_INDEX = 2;
    private static readonly int LAST_OPTION_SPUN_INDEX = 3;
    private static readonly int REMOVE_INDEX = 4;
}
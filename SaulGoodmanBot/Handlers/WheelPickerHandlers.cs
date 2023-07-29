using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class WheelPickerHandlers {
    public static async Task HandleAdd(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "adddropdown") {
            var intr = s.GetInteractivity();
            var wheelName = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild.Id);
            var optionsAdded = new List<string>();
            var description = "Enter `stop` to stop adding options\n";
            var optionCount = 1;

            // display starting message
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, 
                new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Adding to {wheelName}...")
                    .WithDescription(description))));
            var response = await intr.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));

            // continue to add options to wheel
            while (!response.Result.Content.ToLower().Contains("stop") && optionCount <= 10) {
                // add option and update messages
                optionsAdded.Add(response.Result.Content);
                description += $"`{optionCount}.` {response.Result.Content}\n";
                await e.Channel.DeleteMessageAsync(response.Result);
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Adding to {wheelName}...")
                        .WithDescription(description)));
                
                // get next option and update count
                response = await intr.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));
                optionCount++;
            }

            // add options to database and display result message
            serverWheels.Add(new Wheel(wheelName, optionsAdded, serverWheels.Wheels[wheelName].Image));
            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Added to {wheelName}")
                    .WithDescription(description.Replace("Enter `stop` to stop adding options\n", String.Empty))
                    .WithColor(DiscordColor.Green)));
        }

        s.ComponentInteractionCreated -= HandleAdd;
    }

    public static async Task HandleSpin(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "spindropdown") {
            var wheelName = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild.Id);

            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("spindropdown", "Select a wheel", wheelOptions, false);

            // spin wheel and display result
            var response = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Spinning {wheelName}...")
                    .WithDescription($"## {serverWheels.Wheels[wheelName].Spin()}")
                    .WithThumbnail(serverWheels.Wheels[wheelName].Image)
                    .WithColor(DiscordColor.Gold))
                .AddComponents(wheelDropdown);
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(response));
        }
    }

    public static async Task HandleList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "listdropdown") {
            var wheelName = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild.Id);

            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("listdropdown", "Select a wheel", wheelOptions, false);

            // add wheel options to description
            var description = String.Empty;
            var optNum = 1;
            foreach (var o in serverWheels.Wheels[wheelName].Options) {
                description += $"`{optNum}`. {o}\n";
                optNum++;
            }

            // display response
            var response = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
                    .WithTitle(serverWheels.Wheels[wheelName].Name)
                    .WithThumbnail(serverWheels.Wheels[wheelName].Image)
                    .WithDescription(description)
                    .WithColor(DiscordColor.MidnightBlue))
                .AddComponents(wheelDropdown);
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(response));
        }
    }
}
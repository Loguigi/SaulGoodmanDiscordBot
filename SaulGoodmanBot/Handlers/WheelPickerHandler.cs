using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class WheelPickerHandler {
    public static async Task HandleAdd(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "adddropdown") {
            var intr = s.GetInteractivity();
            var wheelName = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild);
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
            var serverWheels = new WheelPickers(e.Guild);

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

    public static async Task HandleDelete(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "deletewheeldropdown") {
            var serverWheels = new WheelPickers(e.Guild);
            var wheelName = e.Values.First();

            // add wheel options to dropdown
            var optionsList = new List<DiscordSelectComponentOption>();
            foreach (var option in serverWheels.Wheels[wheelName].Options) {
                optionsList.Add(new DiscordSelectComponentOption(option, $"{wheelName}/{option}"));
            }
            optionsList.Add(new DiscordSelectComponentOption($"Delete {wheelName} from {e.Guild.Name}", $"{wheelName}/deletetheentirewheel", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":x:", false))));
            optionsList.Add(new DiscordSelectComponentOption("Cancel", "cancel", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":arrow_backward:", false))));
            var wheelDropdown = new DiscordSelectComponent("deleteoptiondropdown", "Select an option", optionsList, false);

            // display next prompt for wheel option
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Delete Wheel Option")
                    .WithDescription($"Deleting from `{wheelName}` wheel...")
                    .WithColor(DiscordColor.DarkRed))
                .AddComponents(wheelDropdown);
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(prompt));
        } else if (e.Id == "deleteoptiondropdown") {
            var serverWheels = new WheelPickers(e.Guild);
            var select = e.Values.First();

            if (select == "cancel") {
                // cancel operation
                var prompt = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Delete Wheel Option")
                        .WithDescription($"Cancelled")
                        .WithColor(DiscordColor.DarkRed));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(prompt));
            } else if (select.Contains("deletetheentirewheel")) {
                // delete wheel
                var wheelName = select[..select.IndexOf('/')];

                // update message
                var prompt = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Delete Wheel")
                        .WithDescription($"Deleted `{wheelName}` from {e.Guild.Name}")
                        .WithColor(DiscordColor.DarkRed));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(prompt));

                // delete wheel from database
                serverWheels.Delete(serverWheels.Wheels[wheelName]);
            } else {
                // delete wheel option
                var wheelName = select[..select.IndexOf('/')];
                var wheelOption = select.Replace(wheelName + "/", string.Empty);

                // update message
                var prompt = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Delete Wheel Option")
                        .WithDescription($"Deleted {wheelOption} from `{wheelName}`")
                        .WithColor(DiscordColor.DarkRed));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(prompt));
            
                // delete wheel option from database
                serverWheels.Delete(serverWheels.Wheels[wheelName], wheelOption);
            }

            s.ComponentInteractionCreated -= HandleDelete;
        }
    }

    public static async Task HandleList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "listdropdown") {
            var wheelName = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild);

            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("listdropdown", "Select a wheel", wheelOptions, false);

            // add wheel options to description
            var description = string.Empty;
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
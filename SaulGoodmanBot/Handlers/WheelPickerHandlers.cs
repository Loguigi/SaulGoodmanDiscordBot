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
            var wheel = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild.Id);
            var optionsAdded = new List<string>();
            var description = "Enter `stop` to stop adding options\n";
            var optionCount = 1;

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, 
                new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Adding to {wheel}...")
                    .WithDescription(description))));

            var response = await intr.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));

            while (!response.Result.Content.ToLower().Contains("stop") && optionCount <= 10) {
                optionsAdded.Add(response.Result.Content);
                description += $"`{optionCount}.` {response.Result.Content}\n";

                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Adding to {wheel}...")
                        .WithDescription(description)));
                await e.Channel.DeleteMessageAsync(response.Result);
                response = await intr.WaitForMessageAsync(u => u.Channel == e.Channel && u.Author == e.User, TimeSpan.FromSeconds(60));
                optionCount++;
            }
            serverWheels.Add(new Wheel(wheel, optionsAdded, serverWheels.Wheels[wheel].Image));

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"Added to {wheel}")
                    .WithDescription(description.Replace("Enter `stop` to stop adding options\n", String.Empty))
                    .WithColor(DiscordColor.Green)));
        }

        s.ComponentInteractionCreated -= HandleAdd;
    }

    public static async Task HandleSpin(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "spindropdown") {
            var wheel = e.Values.First();
            var serverWheels = new WheelPickers(e.Guild.Id);
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(serverWheels.Wheels[wheel].Spin()));
        }
        
        s.ComponentInteractionCreated -= HandleSpin;
    }
}
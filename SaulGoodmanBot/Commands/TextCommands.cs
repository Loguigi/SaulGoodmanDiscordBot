/*
    TextCommands.cs

    Text commands for the bot

    *Slash Commands:
    -flip: flip a coin
    -8ball: asks question to magic 8 ball
*/

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using SaulGoodmanBot.Library;


namespace SaulGoodmanBot.Commands;

public class TextCommands : ApplicationCommandModule {
    [SlashCommand("flip", "Flips a coin")]
    public async Task CoinFlipCommand(InteractionContext cmd) {
        var coin = new Random();
        int flip = coin.Next(1, 3);
        var response = new DiscordEmbedBuilder() {
            Title = (flip == 1) ? "HEADS" : "TAILS",
            Description = "Coin Flip Results",
            Color = (flip == 1) ? DiscordColor.Aquamarine : DiscordColor.Rose
        };

        await cmd.CreateResponseAsync(response);
    }

    [SlashCommand("8ball", "Ask the magic 8 ball a question")]
    public async Task Magic8BallCommand(InteractionContext cmd, [Option("question", "Question to ask the 8 ball")] string question) {
        if (question == "") await cmd.CreateResponseAsync("You must provide a question");
        else {
            var answer = new DiscordEmbedBuilder() {
                Title = $"{cmd.Member.DisplayName} asked '{question}'",
                Description = $"Answer: {Magic8Ball.GetAnswer()}",
                Color = DiscordColor.Azure,
            };

            await cmd.CreateResponseAsync(answer);
        }
    }
}
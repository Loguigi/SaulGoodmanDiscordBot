/*
    TextCommands.cs

    Text commands for the bot

    *Commands:
    -flip: flip a coin
*/

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Commands;

public class TextCommands : BaseCommandModule {
    [Command("flip")]
    public async Task CoinFlip(CommandContext cmd) {
        var coin = new Random();
        int flip = coin.Next(1, 3);
        var response = new DiscordEmbedBuilder() {
            Title = (flip == 1) ? "HEADS" : "TAILS",
            Description = "Coin Flip Results",
            Color = (flip == 1) ? DiscordColor.Aquamarine : DiscordColor.Rose
        };

        await cmd.RespondAsync(response);
    }

    [Command("8ball")]
    public async Task Magic8Ball(CommandContext cmd, string question="") {
        if (question == "") await cmd.RespondAsync("You must provide a question");
        else {
            
        }
    }
}

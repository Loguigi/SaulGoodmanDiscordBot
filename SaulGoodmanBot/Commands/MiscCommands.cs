/*
    TextCommands.cs

    Text commands for the bot

    *Slash Commands:
    -flip: flip a coin
    -8ball: asks question to magic 8 ball
*/

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;


namespace SaulGoodmanBot.Commands;

public class MiscCommands : ApplicationCommandModule {
    [SlashCommand("flip", "Flips a coin")]
    public async Task CoinFlipCommand(InteractionContext cmd) {
        var coin = new Random();
        int flip = coin.Next(1, 3);
        var response = new DiscordEmbedBuilder()
            .WithDescription($"# {((flip == 1) ? "Heads" : "Tails")}")
            .WithColor((flip == 1) ? DiscordColor.Aquamarine : DiscordColor.Rose);

        await cmd.CreateResponseAsync(response);
    }

    [SlashCommand("8ball", "Ask the magic 8 ball a question")]
    public async Task Magic8BallCommand(InteractionContext cmd, [Option("question", "Question to ask the 8 ball")] string question) {
        if (question == "") await cmd.CreateResponseAsync("You must provide a question");
        else {
            var answer = new DiscordEmbedBuilder()
                .WithAuthor($"{cmd.User.GlobalName} asked...", "", cmd.User.AvatarUrl)
                .WithTitle($"\"{question}\"")
                .WithThumbnail(ImageHelper.Images["Heisenberg"])
                .WithDescription($"> {Magic8Ball.GetAnswer()}")
                .WithColor(DiscordColor.Azure)
                .WithTimestamp(DateTimeOffset.Now);

            await cmd.CreateResponseAsync(answer);
        }
    }
}
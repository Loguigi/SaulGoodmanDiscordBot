/*
    MiscCommands.cs

    Misc commands for fun

    *Command List*
    -flip: flip a coin
    -8ball: asks question to magic 8 ball
*/

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

public class MiscCommands : ApplicationCommandModule {
    [SlashCommand("flip", "Flips a coin")]
    public async Task CoinFlipCommand(InteractionContext ctx) {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(CoinFlip()));
    }

    [SlashCommand("8ball", "Ask the magic 8 ball a question")]
    public async Task Magic8BallCommand(InteractionContext ctx, [Option("question", "Question to ask the 8 ball")] string question) {
        var answer = new DiscordEmbedBuilder()
            .WithAuthor("Magic 8Ball", "", ImageHelper.Images["8ball"])
            .AddField("Question", question)
            .AddField("Answer", Magic8Ball.GetAnswer())
            .WithThumbnail(ImageHelper.Images["Heisenberg"])
            .WithColor(DiscordColor.DarkBlue);

        await ctx.CreateResponseAsync(answer);
    }

    public DiscordMessageBuilder CoinFlip() {
        var flipButton = new DiscordButtonComponent(ButtonStyle.Success, "flipagain", "Flip Again");
        var coin = new Random();
        int flip = coin.Next(1, 3);
        var response = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithAuthor("Coin Flip", "", ImageHelper.Images["Coin"])
                .WithDescription($"# {((flip == 1) ? "Heads" : "Tails")}")
                .WithThumbnail(ImageHelper.Images["PS2Jesse"])
                .WithColor((flip == 1) ? DiscordColor.Aquamarine : DiscordColor.Rose))
            .AddComponents(flipButton);
        return response;
    }

    public async Task CoinFlipHandler(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(CoinFlip()));
    }
}
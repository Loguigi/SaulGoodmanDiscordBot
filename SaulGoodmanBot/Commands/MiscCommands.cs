/*
    MiscCommands.cs

    Misc commands for fun

    *Command List*
    -flip: flip a coin
    -8ball: asks question to magic 8 ball
*/

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Handlers;

namespace SaulGoodmanBot.Commands;

public class MiscCommands : ApplicationCommandModule {
    [SlashCommand("flip", "Flips a coin")]
    public async Task CoinFlipCommand(InteractionContext ctx) {
        var coin = new Random();
        int flip = coin.Next(1, 3);
        var flipButton = new DiscordButtonComponent(ButtonStyle.Success, $"{IDHelper.Misc.FLIP}\\{(flip == 1 ? "Heads" : "Tails")}\\{(flip == 1 ? "1" : "0")}\\{(flip == 1 ? "0" : "1")}", "Flip Again");

        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Coin Flip", "", ImageHelper.Images["Coin"])
            .WithDescription($"# {((flip == 1) ? "Heads" : "Tails")}")
            .WithThumbnail(ImageHelper.Images["PS2Jesse"])
            .WithColor((flip == 1) ? DiscordColor.Aquamarine : DiscordColor.Rose);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(flipButton)));


        ctx.Client.ComponentInteractionCreated -= MiscHandler.HandleFlip;
        ctx.Client.ComponentInteractionCreated += MiscHandler.HandleFlip;
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

    [SlashCommand("dicksize", "Finds the penis size of the specified user")]
    public async Task SchlongSize(InteractionContext ctx,
        [Option("user", "User to find the size of")] DiscordUser user) {
        
        // calculate size
        var random = new Random();
        var inches = random.NextDouble() * (10 - 1) + 1;

        // display result
        var response = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithDescription($"## {user.Mention}'s penis is {string.Format("{0:0.##}", inches)} in. long")
                .WithColor(DiscordColor.IndianRed));
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(response));
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
}
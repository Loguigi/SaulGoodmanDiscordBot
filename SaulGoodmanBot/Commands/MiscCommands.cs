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
using SaulGoodmanBot.Helpers;
using SaulGoodmanBot.Handlers;
using DSharpPlus.Interactivity.Extensions;
using System.Reflection;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Commands;

public class MiscCommands : ApplicationCommandModule 
{
    [SlashCommand("flip", "Flips a coin")]
    public async Task CoinFlipCommand(InteractionContext ctx) 
    {
        try 
        {
            int flip = RandomHelper.RNG.Next(2);
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
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("8ball", "Ask the magic 8 ball a question")]
    public async Task Magic8BallCommand(InteractionContext ctx, [Option("question", "Question to ask the 8 ball")] string question) 
    {
        try 
        {
            var answer = new DiscordEmbedBuilder()
                .WithAuthor("Magic 8Ball", "", ImageHelper.Images["8ball"])
                .AddField("Question", question)
                .AddField("Answer", Magic8Ball.GetAnswer())
                .WithThumbnail(ImageHelper.Images["Heisenberg"])
                .WithColor(DiscordColor.DarkBlue);

            await ctx.CreateResponseAsync(answer);
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("rng", "Random number generator")]
    public async Task RNGCommand(InteractionContext ctx,
        [Option("minimum", "Minimum number")][Minimum(0)] long minimum,
        [Option("maximum", "Maximum number")][Minimum(0)] long maximum) 
    {
        
        var result = RandomHelper.RNG.Next((int)minimum, (int)maximum + 1);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Random Number")
            .WithDescription($"# `{result}`")
            .AddField("Minimum", minimum.ToString(), true)
            .AddField("Maximum", maximum.ToString(), true)
            .WithColor(DiscordColor.Lilac);

        await ctx.CreateResponseAsync(embed);
    }

    [SlashCommand("roll", "Roll a dice or multiple dice")]
    public async Task DiceRoll(InteractionContext ctx, 
        [Option("dice_type", "Type of dice to roll")] DiceTypes type=DiceTypes.D6,
        [Option("dice_count", "Number of dice to roll")][Minimum(1)][Maximum(10)] long diceCount=1) 
    {
        var dice = new Dice(type);

        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.White)
            .WithFooter("Total Roll: ");
        
        embed.WithTitle(dice.Type switch 
        {
            DiceTypes.D4 => "D4 Roll",
            DiceTypes.D6 => "D6 Roll",
            DiceTypes.D8 => "D8 Roll",
            DiceTypes.D10 => "D10 Roll",
            DiceTypes.D100 => "D100 Roll",
            DiceTypes.D12 => "D12 Roll",
            DiceTypes.D20 => "D20 Roll",
            _ => ""
        });

        if (diceCount == 1) 
        {
            var roll = dice.RollOnce();
            embed.WithDescription($"## `{roll}`");
            embed.Footer.Text += roll.ToString();
        } 
        else 
        {
            var rolls = dice.RollMultiple((int)diceCount);
            for (var i = 0; i < rolls.Count; ++i) {
                embed.AddField($"Dice #{i + 1}", $"`{rolls[i]}`", true);
            }
            embed.Footer.Text += dice.GetRollTotal(rolls).ToString();
        }

        await ctx.CreateResponseAsync(embed);
    }
}
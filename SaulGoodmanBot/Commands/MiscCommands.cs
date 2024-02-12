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
using SaulGoodmanBot.Controllers;
using DSharpPlus.Interactivity.Extensions;
using System.Reflection;

namespace SaulGoodmanBot.Commands;

public class MiscCommands : ApplicationCommandModule {
    [SlashCommand("flip", "Flips a coin")]
    public async Task CoinFlipCommand(InteractionContext ctx) {
        try {
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
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("8ball", "Ask the magic 8 ball a question")]
    public async Task Magic8BallCommand(InteractionContext ctx, [Option("question", "Question to ask the 8 ball")] string question) {
        try {
            var answer = new DiscordEmbedBuilder()
                .WithAuthor("Magic 8Ball", "", ImageHelper.Images["8ball"])
                .AddField("Question", question)
                .AddField("Answer", Magic8Ball.GetAnswer())
                .WithThumbnail(ImageHelper.Images["Heisenberg"])
                .WithColor(DiscordColor.DarkBlue);

            await ctx.CreateResponseAsync(answer);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("poll", "Starts a poll in the server")]
    public async Task PollCommand(InteractionContext ctx,
        [Option("question", "Question to ask the server")][MaximumLength(256)] string question,
        [Choice("1 minute", 1)]
        [Choice("5 minutes", 5)]
        [Choice("10 minutes", 10)]
        [Choice("30 minutes", 30)]
        [Choice("1 hour", 60)]
        [Option("time_limit", "Time limit to set for the poll")] long time_limit,
        [Option("options", "Number of options to add (can be up to 10)")][Minimum(2)][Maximum(10)] long num_options) {
        
        try {
            var poll = new Poll(ctx.Client, question, TimeSpan.FromMinutes(time_limit), (int)num_options);
            var interactivity = ctx.Client.GetInteractivity();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.User.GlobalName, "", ctx.User.AvatarUrl)
                .WithTitle($"\"{poll.Title}\"")
                .AddField("Last Added", "---")
                .AddField("Options Left", num_options.ToString())
                .WithFooter("Adding options...");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            var response = await interactivity.WaitForMessageAsync(u => u.Channel == ctx.Channel && u.Author == ctx.User, TimeSpan.FromSeconds(60));

            while (num_options > 0 && !response.TimedOut) {
                num_options--;

                var option = response.Result.Content;
                poll.AddOption(option);
                embed.Fields.Where(x => x.Name == "Last Added").First().Value = option;
                embed.Fields.Where(x => x.Name == "Options Left").First().Value = num_options.ToString();

                await ctx.Channel.DeleteMessageAsync(response.Result);
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                if (num_options == 0)
                    break;

                response = await interactivity.WaitForMessageAsync(u => u.Channel == ctx.Channel && u.Author == ctx.User, TimeSpan.FromSeconds(60));
            }

            embed.RemoveFieldRange(0, 2);

            if (response.TimedOut) {
                embed.WithFooter("Poll cancelled").WithColor(DiscordColor.DarkRed);
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                return;
            }

            foreach (var o in poll.Options) {
                embed.Description += $"### {o.Emoji} - {o.Name}\n";
            }

            embed.WithColor(DiscordColor.Cyan).WithFooter($"Accepting responses for {time_limit} minutes");

            var sent_poll = await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            foreach (var emoji in poll.GetEmojis()) {
                await sent_poll.CreateReactionAsync(emoji);
            }

            var total_reactions = await interactivity.CollectReactionsAsync(sent_poll, poll.TimeLimit);

            foreach (var emoji in total_reactions) {
                poll.Options.Find(x => x.Emoji == emoji.Emoji)!.Votes++;
            }

            embed.Description = string.Empty;
            embed.WithColor(DiscordColor.DarkGreen).WithFooter($"Results - {poll.GetTotalVotes()} total votes");

            poll.Options.Sort(delegate(PollOption x, PollOption y) {return y.Votes.CompareTo(x.Votes);});

            foreach (var o in poll.Options) {
                embed.Description += poll.Options.IndexOf(o) switch {
                    0 => $"### {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {o.Name} `{o.Votes} VOTES` `{poll.FormatPercent(o)}`\n",
                    1 => $"### {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {o.Name} `{o.Votes} VOTES` `{poll.FormatPercent(o)}`\n",
                    2 => $"### {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {o.Name} `{o.Votes} VOTES` `{poll.FormatPercent(o)}`\n",
                    _ => $"### {o.Name} `{o.Votes} VOTES` `{poll.FormatPercent(o)}`\n"
                };
            }

            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
}
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Handlers;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Helpers;
using System.Reflection;

namespace SaulGoodmanBot.Commands;

public class LevelCommands : ApplicationCommandModule {
    [SlashCommand("level", "Check your own level or another user's level")]
    public async Task CheckLevel(InteractionContext ctx,
        [Option("user", "User to check the level of")] DiscordUser? user=null) {
        if (user! != null! && user.IsBot) {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        try {
            var level = new Levels(ctx.Guild, user ?? ctx.User);

            var embed = new DiscordEmbedBuilder()
                .AddField("Rank", $"#{level.Rank}", true)
                .AddField("Level", level.Level.ToString(), true)
                .AddField("Experience", $"{level.Experience} / {level.ExpNeededForNextLevel} XP", true)
                .WithThumbnail(level.User.AvatarUrl)
                .WithColor(DiscordColor.Violet);
            
            embed.Description += level.Rank switch {
                1 => $"## {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {level.User.Mention}",
                2 => $"## {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {level.User.Mention}",
                3 => $"## {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {level.User.Mention}",
                _ => $"## {level.User.Mention}"
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
        
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Level")]
    public async Task ContextCheckLevel(ContextMenuContext ctx) {
        if (ctx.TargetUser.IsBot) {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        try {
            var level = new Levels(ctx.Guild, ctx.TargetUser);

            var embed = new DiscordEmbedBuilder()
                .AddField("Rank", $"#{level.Rank}", true)
                .AddField("Level", level.Level.ToString(), true)
                .AddField("Experience", $"{level.Experience} / {level.ExpNeededForNextLevel} XP", true)
                .WithThumbnail(level.User.AvatarUrl)
                .WithColor(DiscordColor.Violet);

            embed.Description += level.Rank switch {
                1 => $"## {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {level.User.Mention}",
                2 => $"## {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {level.User.Mention}",
                3 => $"## {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {level.User.Mention}",
                _ => $"## {level.User.Mention}"
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
        
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
        
    }

    [SlashCommand("leaderboard", "Level ranking of the whole server")]
    public async Task LevelLeaderboard(InteractionContext ctx) {
        try {
            var leaderboard = new List<Levels>();
            foreach (var user in await ctx.Guild.GetAllMembersAsync()) {
                if (!user.IsBot) leaderboard.Add(new Levels(ctx.Guild, user));
            }
            leaderboard.Sort(delegate(Levels x, Levels y) {return x.Rank.CompareTo(y.Rank);});
            var interactivity = new InteractivityHelper<Levels>(ctx.Client, leaderboard, IDHelper.Levels.LEADERBOARD, "1", 10);

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "https://youtu.be/nQGodNKogEI", ctx.Guild.IconUrl)
                .WithTitle("Server Leaderboard")
                .WithDescription("")
                .WithFooter(interactivity.PageStatus)
                .WithColor(DiscordColor.Orange);
            foreach (var user in interactivity.GetPage()) {
                embed.Description += user.Rank switch {
                    1 => $"### {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    2 => $"### {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    3 => $"### {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    _ => $"### **__#{user.Rank}__** {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                };
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

            ctx.Client.ComponentInteractionCreated -= LevelHandler.HandleLeaderboard;
            ctx.Client.ComponentInteractionCreated += LevelHandler.HandleLeaderboard;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
}

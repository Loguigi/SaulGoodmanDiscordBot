using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Handlers;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Commands;

public class LevelCommands : ApplicationCommandModule {
    [SlashCommand("level", "Check your own level or another user's level")]
    public async Task CheckLevel(InteractionContext ctx,
        [Option("user", "User to check the level of")] DiscordUser? user=null) {
        
        var level = new Levels(ctx.Guild, user ?? ctx.User, DateTime.Now);

        var message = new DiscordEmbedBuilder()
            .AddField("Rank", $"#{level.GetRank()}", true)
            .AddField("Level", level.Level.ToString(), true)
            .AddField("Experience", $"{level.Experience} / {level.ExpNeededForNextLevel()} XP", true)
            .WithThumbnail(level.User.AvatarUrl)
            .WithColor(DiscordColor.Violet);
        
        switch(level.GetRank()) {
            case 1: message.WithDescription($"## {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {level.User.Mention}"); break;
            case 2: message.WithDescription($"## {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {level.User.Mention}"); break;
            case 3: message.WithDescription($"## {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {level.User.Mention}"); break;
            default: message.WithDescription($"## {level.User.Mention}"); break;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(message)));
    }

    [SlashCommand("leaderboard", "Level ranking of the whole server")]
    public async Task LevelLeaderboard(InteractionContext ctx) {
        var leaderboard = new List<Levels>();
        foreach (var user in await ctx.Guild.GetAllMembersAsync()) {
            if (!user.IsBot) leaderboard.Add(new Levels(ctx.Guild, user, DateTime.Now));
        }
        leaderboard.Sort(delegate(Levels x, Levels y) {return x.GetRank().CompareTo(y.GetRank());});
        var interactivity = new InteractivityHelper<Levels>(ctx.Client, leaderboard, IDHelper.Levels.LEADERBOARD, "1");

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(ctx.Guild.Name, "https://youtu.be/nQGodNKogEI", ctx.Guild.IconUrl)
            .WithTitle("Server Leaderboard")
            .WithDescription("")
            .WithFooter(interactivity.PageStatus())
            .WithColor(DiscordColor.Orange);
        foreach (var user in interactivity.GetPage()) {
            embed.Description += user.GetRank() switch {
                1 => $"### {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                2 => $"### {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                3 => $"### {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                _ => $"### **__#{user.GetRank()}__** {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
            };
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

        ctx.Client.ComponentInteractionCreated -= LevelHandler.HandleLeaderboard;
        ctx.Client.ComponentInteractionCreated += LevelHandler.HandleLeaderboard;
    }
}

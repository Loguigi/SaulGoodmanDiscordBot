using SaulGoodmanBot.Library;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using SaulGoodmanBot.Helpers;
using System.Reflection;

namespace SaulGoodmanBot.Handlers;

public static class LevelHandler {
    public static async Task HandleExpGain(DiscordClient s, MessageCreateEventArgs e) {
        try {
            var config = new ServerConfig(e.Guild);
            if (e.Author.IsBot || !config.EnableLevels) {
                await Task.CompletedTask;
                return;
            }
            var user = new Levels(e.Guild, e.Author) { NewMsgSent = e.Message.CreationTimestamp.LocalDateTime };

            if (user.NewMsgSent >= user.MsgLastSent.AddMinutes(1)) {
                user.GrantExp();
            }

            if (user.LevelledUp) {
                var embed = new DiscordEmbedBuilder()
                    .WithDescription($"### {DiscordEmoji.FromName(s, ":arrow_up:")} {e.Author.Mention} {config.LevelUpMessage}")
                    .WithFooter($"Level {user.Level - 1} {DiscordEmoji.FromName(s, ":arrow_right:")} Level {user.Level}", e.Author.AvatarUrl)
                    .WithColor(DiscordColor.Cyan);
                await config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent(e.Author.Mention).AddMention(new UserMention(e.Author)).AddEmbed(embed));
            }
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public static async Task HandleLeaderboard(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Levels.LEADERBOARD)) {
            await Task.CompletedTask;
            return;
        }

        try {
            var leaderboard = new List<Levels>();
            await foreach (var user in e.Guild.GetAllMembersAsync()) {
                if (!user.IsBot) leaderboard.Add(new Levels(e.Guild, user));
            }
            leaderboard.Sort(delegate(Levels x, Levels y) {return x.Rank.CompareTo(y.Rank);});
            var interactivity = new InteractivityHelper<Levels>(s, leaderboard, IDHelper.Levels.LEADERBOARD, e.Id.Split('\\')[PAGE_INDEX], 10);

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(e.Guild.Name, "https://youtu.be/nQGodNKogEI", e.Guild.IconUrl)
                .WithTitle("Server Leaderboard")
                .WithDescription("")
                .WithFooter(interactivity.PageStatus)
                .WithColor(DiscordColor.Orange);
            foreach (var user in interactivity) {
                embed.Description += user.Rank switch {
                    1 => $"### {DiscordEmoji.FromName(s, ":first_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    2 => $"### {DiscordEmoji.FromName(s, ":second_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    3 => $"### {DiscordEmoji.FromName(s, ":third_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    _ => $"### **__#{user.Rank}__** {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                };
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private const int PAGE_INDEX = 1;
}

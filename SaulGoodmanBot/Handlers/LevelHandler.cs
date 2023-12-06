using SaulGoodmanBot.Library;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class LevelHandler {
    public static async Task HandleExpGain(DiscordClient s, MessageCreateEventArgs e) {
        var config = new ServerConfig(e.Guild);
        if (e.Author.IsBot || !config.EnableLevels) {
            await Task.CompletedTask;
            return;
        }
        var user = new Levels(e.Guild, e.Author, e.Message.CreationTimestamp.LocalDateTime);

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
    }

    public static async Task HandleLeaderboard(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Levels.LEADERBOARD)) {
            await Task.CompletedTask;
            return;
        }

        var leaderboard = new List<Levels>();
        foreach (var user in await e.Guild.GetAllMembersAsync()) {
            if (!user.IsBot) leaderboard.Add(new Levels(e.Guild, user, DateTime.Now));
        }
        leaderboard.Sort(delegate(Levels x, Levels y) {return x.GetRank().CompareTo(y.GetRank());});
        var interactivity = new InteractivityHelper<Levels>(s, leaderboard, IDHelper.Levels.LEADERBOARD, e.Id.Split('\\')[PAGE_INDEX]);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "https://youtu.be/nQGodNKogEI", e.Guild.IconUrl)
            .WithTitle("Server Leaderboard")
            .WithDescription("")
            .WithFooter(interactivity.PageStatus())
            .WithColor(DiscordColor.Orange);
        foreach (var user in interactivity.GetPage()) {
            embed.Description += user.GetRank() switch {
                1 => $"### {DiscordEmoji.FromName(s, ":first_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                2 => $"### {DiscordEmoji.FromName(s, ":second_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                3 => $"### {DiscordEmoji.FromName(s, ":third_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                _ => $"### **__#{user.GetRank()}__** {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
            };
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
    }

    private const int PAGE_INDEX = 1;
}

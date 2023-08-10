using SaulGoodmanBot.Library;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;

namespace SaulGoodmanBot.Handlers;

public static class LevelHandler {
    public static async Task HandleExpGain(DiscordClient s, MessageCreateEventArgs e) {
        if (!e.Author.IsBot) {
            var user = new Levels(e.Guild, e.Author, e.Message.CreationTimestamp.LocalDateTime);

            if (user.NewMsgSent >= user.MsgLastSent.AddMinutes(1)) {
                user.GrantExp();

                if (user.LevelledUp) {
                    var message = new DiscordEmbedBuilder()
                        .WithDescription($"### {DiscordEmoji.FromName(s, ":arrow_up:")} {e.Author.Mention} has levelled up!")
                        .WithFooter($"Level {user.Level - 1} {DiscordEmoji.FromName(s, ":arrow_right:")} Level {user.Level}", e.Author.AvatarUrl)
                        .WithColor(DiscordColor.Cyan);
                    await e.Channel.SendMessageAsync(message);
                }
            }
        }
    }
}

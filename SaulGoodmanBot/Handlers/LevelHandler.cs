using SaulGoodmanBot.Library;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;

namespace SaulGoodmanBot.Handlers;

public static class LevelHandler {
    public static async Task HandleExpGain(DiscordClient s, MessageCreateEventArgs e) {
        var user = new Levels(e.Guild, e.Author);

        if (e.Message.CreationTimestamp >= user.MsgLastSent.AddMinutes(1)) {
            user.MsgLastSent = e.Message.CreationTimestamp.DateTime;
            user.GrantExp();

            if (user.LevelledUp) {
                var message = new DiscordEmbedBuilder()
                    .WithDescription($"## {DiscordEmoji.FromName(s, ":arrow_up")} {e.Author.Mention} has levelled up! ({user.Level})")
                    .WithColor(DiscordColor.Cyan);
                await e.Channel.SendMessageAsync(message);
            }
        }
    }
}

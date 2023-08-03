using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class BirthdayHandler {
    public static async Task HandleBirthdayMessage(DiscordClient s, MessageCreateEventArgs e) {
        var bdayList = new Birthdays(e.Guild, s);
        if (!e.Author.IsBot) {
            var config = new ServerConfig(e.Guild);
            if (config.PauseBdayNotifsTimer == DateTime.Now.AddDays(-1)) {
                config.PauseBdayNotifsTimer = bdayList.DATE_ERROR;
                config.UpdateConfig();
            }

            if (config.BirthdayNotifications && config.PauseBdayNotifsTimer == bdayList.DATE_ERROR) {
                foreach (var birthday in bdayList.BirthdayList) {
                    if (birthday.IsBirthdayToday()) {
                        _ = await new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithDescription($"# {DiscordEmoji.FromName(s, ":birthday:", false)} It's the birthday of {birthday.User.Mention}! ({birthday.GetAge()})")
                                .WithColor(DiscordColor.HotPink))
                            .SendAsync(config.DefaultChannel);

                        config.PauseBdayNotifsTimer = DateTime.Now;
                        config.UpdateConfig();
                    }
                }
            }
        }
    }
}
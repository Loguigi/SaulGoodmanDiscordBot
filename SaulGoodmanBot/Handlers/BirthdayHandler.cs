using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class BirthdayHandler {
    public static async Task HandleBirthdayMessage(DiscordClient s, MessageCreateEventArgs e) {
        var bdayList = new Birthdays(e.Guild.Id, s);
        if (!e.Author.IsBot) {
            var Config = new ServerConfig(e.Guild.Id);
            if (Config.PauseBdayNotifsTimer == DateTime.Now.AddDays(-1)) {
                Config.PauseBdayNotifsTimer = bdayList.DATE_ERROR;
                Config.UpdateConfig();
            }

            if (Config.BirthdayNotifications && Config.PauseBdayNotifsTimer == bdayList.DATE_ERROR) {
                foreach (var birthday in bdayList.GetBirthdays()) {
                    if (birthday.IsBirthdayToday()) {
                        var bdayMessage = await new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithDescription($"# {DiscordEmoji.FromName(s, ":birthday:", false)} It's the birthday of {birthday.User.Mention}! ({birthday.GetAge()})")
                                .WithColor(DiscordColor.HotPink))
                            .SendAsync(e.Guild.GetDefaultChannel());

                        Config.PauseBdayNotifsTimer = DateTime.Now;
                        Config.UpdateConfig();
                    }
                }
            }
        }
    }
}
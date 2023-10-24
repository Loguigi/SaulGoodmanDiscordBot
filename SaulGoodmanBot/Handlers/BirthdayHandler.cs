using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Birthdays;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class BirthdayHandler {
    public static async Task HandleBirthdayMessage(DiscordClient s, MessageCreateEventArgs e) {
        var birthdays = new ServerBirthdays(e.Guild);
        var config = new ServerConfig(e.Guild);

        if (e.Author.IsBot || config.PauseBdayNotifsTimer.Date == DateTime.Today || !config.BirthdayNotifications) {
            await Task.CompletedTask;
            return;
        }

        config.PauseBdayNotifsTimer = ServerBirthdays.NO_BIRTHDAY;
        config.UpdateConfig();

        foreach (var birthday in birthdays.GetBirthdays()) {
            if (birthday.IsBirthdayToday()) {
                _ = await new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription($"# {DiscordEmoji.FromName(s, ":birthday:", false)} {config.BirthdayMessage} {birthday.User.Mention} ({birthday.GetAge()})")
                    .WithColor(DiscordColor.HotPink))
                .SendAsync(config.DefaultChannel);

                config.PauseBdayNotifsTimer = DateTime.Now;
                config.UpdateConfig();
            }
        }
    }

    public static async Task HandleBirthdayList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Birthdays.LIST)) {
            await Task.CompletedTask;
            return;
        }

        var birthdays = new ServerBirthdays(e.Guild);
        var interactivity = new InteractivityHelper<Birthday>(s, birthdays.GetBirthdays(), IDHelper.Birthdays.LIST, e.Id.Split('\\')[PAGE_INDEX]);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
            .WithTitle("Birthdays")
            .WithDescription(interactivity.IsEmpty())
            .WithColor(DiscordColor.Magenta)
            .WithFooter(interactivity.PageStatus());

        foreach (var birthday in interactivity.GetPage()) {
            embed.Description += $"### {birthday.User.Mention}: {birthday.BDay:MMMM d} `({birthday.GetAge() + 1})`\n";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
    }

    private const int PAGE_INDEX = 1;
}
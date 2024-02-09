using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Helpers;
using SaulGoodmanBot.Controllers;

namespace SaulGoodmanBot.Handlers;

public static class BirthdayHandler {
    public static async Task HandleBirthdayMessage(DiscordClient s, MessageCreateEventArgs e) {
        var birthdays = new ServerBirthdays(e.Guild);
        var config = new ServerConfig(e.Guild);

        if (e.Author.IsBot || config.PauseBdayNotifsTimer.Date == DateTime.Today || !config.BirthdayNotifications) {
            await Task.CompletedTask;
            return;
        }
        Thread.Sleep(100);

        config.PauseBdayNotifsTimer = DateTime.MinValue;
        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.HotPink);
        
        foreach (var birthday in birthdays) {
            if (birthday.HasBirthdayToday) {
                embed.WithDescription($"# {DiscordEmoji.FromName(s, ":birthday:", false)} {config.BirthdayMessage} {birthday.User.Mention} ({birthday.Age})");
                config.PauseBdayNotifsTimer = DateTime.Now;
                await config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed));
            } else if (birthday.HasUpcomingBirthday) {
                embed.WithDescription($"# {birthday.User.Mention}'s birthday is in **__5__** days!").WithFooter($"{DiscordEmoji.FromName(s, ":birthday:", false)} {birthday} {DiscordEmoji.FromName(s, ":birthday:", false)}");
                config.PauseBdayNotifsTimer = DateTime.Now;
                await config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed));
            }
        }
        config.Save();
    }

    public static async Task HandleBirthdayList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Birthdays.LIST)) {
            await Task.CompletedTask;
            return;
        }

        var birthdays = new ServerBirthdays(e.Guild);
        var interactivity = new InteractivityHelper<Birthday>(s, birthdays.Birthdays, IDHelper.Birthdays.LIST, IDHelper.GetId(e.Id, PAGE_INDEX), 10);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
            .WithTitle("Birthdays")
            .WithDescription(interactivity.IsEmpty())
            .WithColor(DiscordColor.Magenta)
            .WithFooter(interactivity.PageStatus);

        foreach (var birthday in interactivity) {
            embed.Description += $"### {birthday.User.Mention}: {birthday.BDay:MMMM d} `({birthday.Age + 1})`\n";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
    }

    private const int PAGE_INDEX = 1;
}
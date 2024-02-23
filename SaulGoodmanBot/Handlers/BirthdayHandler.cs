using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Helpers;
using SaulGoodmanBot.Controllers;
using System.Reflection;
using System.Timers;

namespace SaulGoodmanBot.Handlers;

public static class BirthdayHandler {
    public static async Task HandleBirthdayMessage(ElapsedEventArgs e) {
        foreach (var guild in Bot.Guilds.Values) {
            if (!guild.Config.BirthdayNotifications)
                continue;
            
            if (e.SignalTime.Hour != guild.Config.BirthdayTimer.Hour)
                continue;

            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.HotPink);

            foreach (var birthday in guild.Birthdays) {
                if (birthday.HasBirthdayToday) {
                    embed.WithDescription($"# {DiscordEmoji.FromName(Bot.Client!, ":birthday:", false)} {guild.Config.BirthdayMessage} {birthday.User.Mention} ({birthday.Age})");
                    await guild.Config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed));
                } else if (birthday.HasUpcomingBirthday) {
                    embed.WithDescription($"# {birthday.User.Mention}'s birthday is in **__5__** days!").WithFooter($"{DiscordEmoji.FromName(Bot.Client!, ":birthday:", false)} {birthday} {DiscordEmoji.FromName(Bot.Client!, ":birthday:", false)}");
                    await guild.Config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed));
                }
            }
        }
    }

    public static async Task HandleBirthdayList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Birthdays.LIST)) {
            await Task.CompletedTask;
            return;
        }

        try {
            var birthdays = new ServerBirthdays(s, e.Guild);
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
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private const int PAGE_INDEX = 1;
}
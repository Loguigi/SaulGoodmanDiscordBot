using System.Drawing;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Helpers;

public static class ScheduleHandler {
    public static async Task HandleTodaysSchedules(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Schedules.Today)) {
            await Task.CompletedTask;
            return;
        }

        var schedules = new List<Schedule>();
        foreach (var user in e.Guild.Members.Values) {
            if (!user.IsBot) {
                schedules.Add(new Schedule(e.Guild, user));
            }
        }
        var interactivity = new InteractivityHelper<Schedule>(s, schedules.Where(x => x.WorkSchedule[DateTime.Now.DayOfWeek] != null).ToList(), IDHelper.Schedules.Today, e.Id.Split('\\')[PAGE_INDEX]);

        var embed = new DiscordEmbedBuilder()
            .WithTitle(DateTime.Now.ToString("dddd MMMM d, yyyy"))
            .WithDescription("")
            .WithColor(DiscordColor.DarkBlue)
            .WithFooter(interactivity.PageStatus());

        foreach (var schedule in interactivity.GetPage()) {
            embed.Description += $"### {schedule.User.Mention}: {schedule.WorkSchedule[DateTime.Now.DayOfWeek]}";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(
            interactivity.AddPageButtons().AddEmbed(embed)
        ));
    }

    private const int PAGE_INDEX = 1;
}
using System.Drawing;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library;

public static class ScheduleHandler {
    public static async Task HandleTodaysSchedules(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Schedules.Today)) {
            await Task.CompletedTask;
            return;
        }

        int page = int.Parse(e.Id.Replace(IDHelper.Schedules.Today, string.Empty));
        var schedules = new List<Schedule>();
        foreach (var user in e.Guild.Members.Values) {
            if (!user.IsBot) {
                schedules.Add(new Schedule(e.Guild, user));
            }
        }
        var interactivity = new InteractivityHelper<Schedule>(s, schedules.Where(x => x.WorkSchedule[DateTime.Now.DayOfWeek] != null).ToList(), IDHelper.Schedules.Today, page);

        var embed = new DiscordEmbedBuilder()
            .WithTitle(DateTime.Now.ToString("dddd MMMM d, yyyy"))
            .WithDescription("")
            .WithColor(DiscordColor.DarkBlue)
            .WithFooter($"Page {interactivity.Page} of {interactivity.PageLimit}");

        foreach (var schedule in interactivity.GetPage()) {
            embed.Description += $"### {schedule.User.Mention}: {schedule.WorkSchedule[DateTime.Now.DayOfWeek]}";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(
            interactivity.AddPageButtons().AddEmbed(embed)
        ));
    }
}
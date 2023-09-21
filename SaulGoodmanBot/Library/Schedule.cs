using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;
using System.Data;

namespace SaulGoodmanBot.Library;

public class Schedule {
    public Schedule(DiscordGuild guild, DiscordUser user) {
        Guild = guild;
        User = user;
        var data = ScheduleProcessor.ViewSchedule(Guild.Id, User.Id);

        if (data == null) {
            ScheduleProcessor.SaveSchedule(new ScheduleModel() { GuildId = (long)Guild.Id, UserId=(long)User.Id });
            return;
        }

        for (var i = DayOfWeek.Sunday; i <= DayOfWeek.Saturday; ++i) {
            WorkSchedule[i] = data.ToList()[(int)i];
        }
        LastUpdated = data.LastUpdated;
        RecurringSchedule = data.RecurringSchedule == 1;
        PictureUrl = data.PictureUrl;
    }

    public void Update() {
        ScheduleProcessor.UpdateSchedule(new ScheduleModel() {
            GuildId = (long)Guild.Id
        });
    }


    private DiscordGuild Guild { get; set; }
    public DiscordUser User { get; private set; }
    public DateTime? LastUpdated { get; set; }
    public bool RecurringSchedule { get; set; } = false;
    public Dictionary<DayOfWeek, string?> WorkSchedule { get; set; } = new() {
        {DayOfWeek.Sunday, null},
        {DayOfWeek.Monday, null},
        {DayOfWeek.Tuesday, null},
        {DayOfWeek.Wednesday, null},
        {DayOfWeek.Thursday, null},
        {DayOfWeek.Friday, null},
        {DayOfWeek.Saturday, null}
    };
    public string? PictureUrl { get; set; } = null;
}
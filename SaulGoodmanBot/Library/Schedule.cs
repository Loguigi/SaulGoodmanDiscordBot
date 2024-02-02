using DSharpPlus.Entities;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Library;

internal class Schedule : DbBase<ScheduleModel, Schedule> {
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
        //LastUpdated = data.LastUpdated ?? NO_DATE;
        RecurringSchedule = data.RecurringSchedule == 1;
        PictureUrl = data.PictureUrl;
    }

    public void Update() {
        ScheduleProcessor.UpdateSchedule(new ScheduleModel() {
            GuildId = (long)Guild.Id,
            UserId = (long)User.Id,
            LastUpdated = LastUpdated,
            RecurringSchedule = RecurringSchedule ? 1 : 0,
            Sunday = WorkSchedule[DayOfWeek.Sunday],
            Monday = WorkSchedule[DayOfWeek.Monday],
            Tuesday = WorkSchedule[DayOfWeek.Tuesday],
            Wednesday = WorkSchedule[DayOfWeek.Wednesday],
            Thursday = WorkSchedule[DayOfWeek.Thursday],
            Friday = WorkSchedule[DayOfWeek.Friday],
            Saturday = WorkSchedule[DayOfWeek.Saturday],
            PictureUrl = PictureUrl
        });
    }

    public void Clear() {
        ScheduleProcessor.UpdateSchedule(new ScheduleModel() {
            GuildId = (long)Guild.Id,
            UserId = (long)User.Id,
            LastUpdated = LastUpdated,
            RecurringSchedule = RecurringSchedule ? 1 : 0
        });
    }

    #region DB Methods
    protected override ResultArgs<List<ScheduleModel>> GetData(string sp)
    {
        throw new NotImplementedException();
    }

    protected override ResultArgs<int> SaveData(string sp, ScheduleModel data)
    {
        throw new NotImplementedException();
    }

    protected override List<Schedule> MapData(List<ScheduleModel> data)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Properties
    private DiscordGuild Guild { get; set; }
    public DiscordUser User { get; private set; }
    public DateTime LastUpdated { get; set; }
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
    public string PictureUrl { get; set; } = string.Empty;
    #endregion
}
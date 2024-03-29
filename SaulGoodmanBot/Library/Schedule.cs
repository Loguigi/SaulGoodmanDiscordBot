using DSharpPlus.Entities;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Data;
using Dapper;
using System.Reflection;

namespace SaulGoodmanBot.Library;

public class Schedule : DbBase<ScheduleModel, Schedule> {
    #region Properties
    private DiscordGuild Guild { get; set; }
    public DiscordUser User { get; private set; }
    public DateTime LastUpdated { get; set; } = Constants.DATE_ERROR;
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
    #endregion
    
    public Schedule(DiscordGuild guild, DiscordUser user) {
        Guild = guild;
        User = user;
        
        try {
            var result = GetData("Schedule_GetData", new DynamicParameters(new { GuildId = (long)Guild.Id, UserId = (long)User.Id })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Update() {
        try {
            var result = SaveData("Schedule_Process", new DynamicParameters(
                new ScheduleModel() {
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
                    PictureUrl = PictureUrl,
                    Mode = (int)DataMode.SAVE
            })).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Clear() {
        try {
            var result = SaveData("Schedule_Process", new DynamicParameters(
                new ScheduleModel() {
                    GuildId = (long)Guild.Id,
                    UserId = (long)User.Id,
                    LastUpdated = LastUpdated,
                    RecurringSchedule = RecurringSchedule ? 1 : 0
            })).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    #region DB Methods
    protected override List<Schedule> MapData(List<ScheduleModel> data)
    {
        var schedule = data.FirstOrDefault();
        if (schedule == null)
            return new List<Schedule>();

        for (var i = DayOfWeek.Sunday; i <= DayOfWeek.Saturday; ++i) {
            WorkSchedule[i] = schedule.ToList()[(int)i];
        }
        LastUpdated = schedule.LastUpdated;
        RecurringSchedule = schedule.RecurringSchedule == 1;
        PictureUrl = schedule.PictureUrl;

        return new List<Schedule>();
    }

    private enum DataMode {
        SAVE = 0,
        CLEAR = 1,
        REMOVE = 2
    }
    #endregion
}
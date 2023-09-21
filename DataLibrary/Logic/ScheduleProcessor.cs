using DataLibrary.Models;
using DataLibrary.Config;

namespace DataLibrary.Logic;

public static class ScheduleProcessor {
    public static int SaveSchedule(ScheduleModel schedule) {
        string sql = @"insert into dbo.Schedules values (@GuildId, @UserId, @LastUpdated, @RecurringSchedule, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, @Saturday, @Sunday, @PictureUrl);";
        return SqlDataAccess.SaveData(sql, schedule);
    }

    public static int UpdateSchedule(ScheduleModel schedule) {
        string sql = @"update dbo.Schedules set LastUpdated=@LastUpdated, RecurringSchedule=@RecurringSchedule, Monday=@Monday, Tuesday=@Tuesday, Wednesday=@Wednesday, Thursday=@Thursday, Friday=@Friday, Saturday=@Saturday, Sunday=@Sunday, PictureUrl=@PictureUrl where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, schedule);
    }

    public static ScheduleModel? ViewSchedule(ulong guildid, ulong userid) {
        string sql = $"select * from dbo.Schedules where GuildId={guildid} and UserId={userid};";
        return SqlDataAccess.LoadData<ScheduleModel>(sql).FirstOrDefault();
    }
}
using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class ConfigProcessor {
    public static List<ConfigModel> LoadConfig(ulong guildid) {
        string sql = @$"select * from dbo.Config where GuildId={guildid};";
        return SqlDataAccess.LoadData<ConfigModel>(sql);
    }

    public static int SaveConfig(ulong guildid, string? welcome=null, string? leave=null, int bdayNotifs=1, DateTime? pauseBdayNotifs=null) {
        string sql = @$"insert into dbo.Config values ({guildid}, '{welcome}', '{leave}', {bdayNotifs}, '{pauseBdayNotifs}');";
        return SqlDataAccess.SaveData<ConfigModel>(sql, new ConfigModel());
    }

    public static int UpdateConfig(ulong guildid, string? welcome=null, string? leave=null, int bdayNotifs=1, DateTime? pauseBdayNotifs=null) {
        string sql = @$"update dbo.Config set 
                WelcomeMessage='{welcome}', 
                LeaveMessage='{leave}', 
                BirthdayNotifications={bdayNotifs}, 
                PauseBdayNotifsTimer='{pauseBdayNotifs}'
            where GuildId={guildid};";
        return SqlDataAccess.SaveData<ConfigModel>(sql, new ConfigModel());
    }
}
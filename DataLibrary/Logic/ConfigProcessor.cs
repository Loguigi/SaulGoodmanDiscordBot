using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class ConfigProcessor {
    public static List<ConfigModel> LoadConfig(ulong guildid) {
        string sql = @$"select * from dbo.Config where GuildId={guildid};";
        return SqlDataAccess.LoadData<ConfigModel>(sql);
    }

    public static int SaveConfig(ulong guildid, string? welcome, string? leave, ulong defaultChannel, int bdayNotifs, DateTime pauseBdayNotifs, string? roleName, string? roleDesc, int allowMultiple) {
        string sql = @$"insert into dbo.Config values ({guildid}, '{welcome}', '{leave}', {defaultChannel}, {bdayNotifs}, '{pauseBdayNotifs}', '{roleName}', '{roleDesc}', {allowMultiple});";
        return SqlDataAccess.SaveData(sql, new ConfigModel());
    }

    public static int UpdateConfig(ulong guildid, string? welcome, string? leave, ulong defaultChannel, int bdayNotifs, DateTime pauseBdayNotifs, string? roleName, string? roleDesc, int allowMultiple) {
        string sql = @$"update dbo.Config set 
                WelcomeMessage='{welcome}', 
                LeaveMessage='{leave}',
                DefaultChannel={defaultChannel}, 
                BirthdayNotifications={bdayNotifs}, 
                PauseBdayNotifsTimer='{pauseBdayNotifs}',
                ServerRolesName='{roleName}',
                ServerRolesDescription='{roleDesc}',
                AllowMultipleRoles={allowMultiple}
            where GuildId={guildid};";
        return SqlDataAccess.SaveData(sql, new ConfigModel());
    }
}
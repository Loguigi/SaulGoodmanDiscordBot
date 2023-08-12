using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class ConfigProcessor {
    public static List<ConfigModel> LoadConfig(ulong guildid) {
        string sql = @$"select * from dbo.Config where GuildId={guildid};";
        return SqlDataAccess.LoadData<ConfigModel>(sql);
    }

    public static int SaveConfig(ulong guildid, string? welcome, string? leave, ulong defaultChannel, int bdayNotifs, DateTime pauseBdayNotifs, string? roleName, string? roleDesc, int allowMultiple, int enablelevels) {
        var config = new ConfigModel() {
            GuildId = (long)guildid,
            WelcomeMessage = welcome,
            LeaveMessage = leave,
            DefaultChannel = (long)defaultChannel,
            BirthdayNotifications = bdayNotifs,
            PauseBdayNotifsTimer = pauseBdayNotifs,
            ServerRolesName = roleName,
            ServerRolesDescription = roleDesc,
            AllowMultipleRoles = allowMultiple,
            EnableLevels = enablelevels
        };
        string sql = @"insert into dbo.Config values (@GuildId, @WelcomeMessage, @LeaveMessage, @DefaultChannel, @BirthdayNotifications, @PauseBdayNotifsTimer, @ServerRolesName, @ServerRolesDescription, @AllowMultipleRoles, @EnableLevels);";
        return SqlDataAccess.SaveData(sql, config);
    }

    public static int UpdateConfig(ulong guildid, string? welcome, string? leave, ulong defaultChannel, int bdayNotifs, DateTime pauseBdayNotifs, string? roleName, string? roleDesc, int allowMultiple, int enablelevels) {
        var config = new ConfigModel() {
            GuildId = (long)guildid,
            WelcomeMessage = welcome,
            LeaveMessage = leave,
            DefaultChannel = (long)defaultChannel,
            BirthdayNotifications = bdayNotifs,
            PauseBdayNotifsTimer = pauseBdayNotifs,
            ServerRolesName = roleName,
            ServerRolesDescription = roleDesc,
            AllowMultipleRoles = allowMultiple,
            EnableLevels = enablelevels
        };
        string sql = @"update dbo.Config set 
                WelcomeMessage=@WelcomeMessage, 
                LeaveMessage=@LeaveMessage,
                DefaultChannel=@DefaultChannel, 
                BirthdayNotifications=@BirthdayNotifications, 
                PauseBdayNotifsTimer=@PauseBdayNotifsTimer,
                ServerRolesName=@ServerRolesName,
                ServerRolesDescription=@ServerRolesDescription,
                AllowMultipleRoles=@AllowMultipleRoles,
                EnableLevels=@EnableLevels
            where GuildId=@GuildId;";
        return SqlDataAccess.SaveData(sql, config);
    }
}
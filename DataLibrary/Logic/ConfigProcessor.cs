using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class ConfigProcessor {
    public static ConfigModel? LoadConfig(ulong guildid) {
        string sql = @$"select * from dbo.Config where GuildId={guildid};";
        return SqlDataAccess.LoadData<ConfigModel>(sql).FirstOrDefault();
    }

    public static int SaveConfig(ulong guildid, string? welcome, string? leave, ulong defaultChannel, int bdayNotifs, DateTime pauseBdayNotifs, string bdayMsg, string? roleName, string? roleDesc, int allowMultiple, int menuOnJoin, int enablelevels, string lvlUpMsg) {
        var config = new ConfigModel() {
            GuildId = (long)guildid,
            WelcomeMessage = welcome,
            LeaveMessage = leave,
            DefaultChannel = (long)defaultChannel,
            BirthdayNotifications = bdayNotifs,
            PauseBdayNotifsTimer = pauseBdayNotifs,
            BirthdayMessage = bdayMsg,
            ServerRolesName = roleName,
            ServerRolesDescription = roleDesc,
            AllowMultipleRoles = allowMultiple,
            SendRoleMenuOnMemberJoin = menuOnJoin,
            EnableLevels = enablelevels,
            LevelUpMessage = lvlUpMsg
        };
        string sql = @"insert into dbo.Config values (@GuildId, @WelcomeMessage, @LeaveMessage, @DefaultChannel, @BirthdayNotifications, @PauseBdayNotifsTimer, @BirthdayMessage, @ServerRolesName, @ServerRolesDescription, @AllowMultipleRoles, @SendRoleMenuOnMemberJoin, @EnableLevels, @LevelUpMessage);";
        return SqlDataAccess.SaveData(sql, config);
    }

    public static int UpdateConfig(ulong guildid, string? welcome, string? leave, ulong defaultChannel, int bdayNotifs, DateTime pauseBdayNotifs, string bdayMsg, string? roleName, string? roleDesc, int allowMultiple, int menuOnJoin, int enablelevels, string lvlUpMsg) {
        var config = new ConfigModel() {
            GuildId = (long)guildid,
            WelcomeMessage = welcome,
            LeaveMessage = leave,
            DefaultChannel = (long)defaultChannel,
            BirthdayNotifications = bdayNotifs,
            PauseBdayNotifsTimer = pauseBdayNotifs,
            BirthdayMessage = bdayMsg,
            ServerRolesName = roleName,
            ServerRolesDescription = roleDesc,
            AllowMultipleRoles = allowMultiple,
            SendRoleMenuOnMemberJoin = menuOnJoin,
            EnableLevels = enablelevels,
            LevelUpMessage = lvlUpMsg
        };
        string sql = @"update dbo.Config set 
                WelcomeMessage=@WelcomeMessage, 
                LeaveMessage=@LeaveMessage,
                DefaultChannel=@DefaultChannel, 
                BirthdayNotifications=@BirthdayNotifications, 
                PauseBdayNotifsTimer=@PauseBdayNotifsTimer,
                BirthdayMessage=@BirthdayMessage,
                ServerRolesName=@ServerRolesName,
                ServerRolesDescription=@ServerRolesDescription,
                AllowMultipleRoles=@AllowMultipleRoles,
                SendRoleMenuOnMemberJoin=@SendRoleMenuOnMemberJoin,
                EnableLevels=@EnableLevels,
                LevelUpMessage=@LevelUpMessage
            where GuildId=@GuildId;";
        return SqlDataAccess.SaveData(sql, config);
    }
}
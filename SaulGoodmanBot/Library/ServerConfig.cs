using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class ServerConfig {
    public ServerConfig(DiscordGuild guild) {
        Guild = guild;
        DefaultChannel = Guild.GetDefaultChannel();
        
        if (config == null) {
            SaveNewServerConfig();
            return;
        }

        WelcomeMessage = config.WelcomeMessage;
        LeaveMessage = config.LeaveMessage;
        DefaultChannel = Guild.GetChannel((ulong)config.DefaultChannel);
        BirthdayNotifications = config.BirthdayNotifications == 1;
        PauseBdayNotifsTimer = config.PauseBdayNotifsTimer;
        BirthdayMessage = config.BirthdayMessage;
        ServerRolesName = config.ServerRolesName;
        ServerRolesDescription = config.ServerRolesDescription;
        AllowMultipleRoles = config.AllowMultipleRoles == 1;
        SendRoleMenuOnMemberJoin = config.SendRoleMenuOnMemberJoin == 1;
        EnableLevels = config.EnableLevels == 1;
        LevelUpMessage = config.LevelUpMessage;
    }

    private void SaveNewServerConfig() {
        ConfigProcessor.SaveConfig(Guild.Id, WelcomeMessage, LeaveMessage, DefaultChannel.Id, BirthdayNotifications ? 1 : 0, PauseBdayNotifsTimer, BirthdayMessage, ServerRolesName, ServerRolesDescription, AllowMultipleRoles ? 1 : 0, SendRoleMenuOnMemberJoin ? 1 : 0, EnableLevels ? 1 : 0, LevelUpMessage);
    }

    public void UpdateConfig() {
        ConfigProcessor.UpdateConfig(Guild.Id, WelcomeMessage, LeaveMessage, DefaultChannel.Id, BirthdayNotifications ? 1 : 0, PauseBdayNotifsTimer, BirthdayMessage, ServerRolesName, ServerRolesDescription, AllowMultipleRoles ? 1 : 0, SendRoleMenuOnMemberJoin ? 1 : 0, EnableLevels ? 1 : 0, LevelUpMessage);
    }

    // Config Properties
    private DiscordGuild Guild { get; set; }

    // General Config
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public DiscordChannel DefaultChannel { get; set; }

    // Birthday Config
    public static DateTime DATE_ERROR { get; private set; } = DateTime.Parse("1/1/1800");
    public bool BirthdayNotifications { get; set; } = true;
    public string BirthdayMessage { get; set; } = "Happy Birthday!";
    public DateTime PauseBdayNotifsTimer { get; set; } = DATE_ERROR;

    // Role Config
    public string? ServerRolesName { get; set; } = null;
    public string? ServerRolesDescription { get; set; } = null;
    public bool AllowMultipleRoles { get; set; } = false;
    public bool SendRoleMenuOnMemberJoin { get; set; } = false;

    // Levels Config
    public bool EnableLevels { get; set; } = false;
    public string LevelUpMessage { get; set; } = "has levelled up!";
}

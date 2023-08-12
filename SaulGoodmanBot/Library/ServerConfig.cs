using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class ServerConfig {
    public ServerConfig(DiscordGuild guild) {
        Guild = guild;
        var data = ConfigProcessor.LoadConfig(Guild.Id);
        DefaultChannel = Guild.GetDefaultChannel();
        
        if (data.Count == 0) {
            SaveNewServerConfig();
        } else {
            foreach (var row in data) {
                WelcomeMessage = row.WelcomeMessage;
                LeaveMessage = row.LeaveMessage;
                DefaultChannel = Guild.GetChannel((ulong)row.DefaultChannel);
                BirthdayNotifications = row.BirthdayNotifications == 1;
                PauseBdayNotifsTimer = row.PauseBdayNotifsTimer;
                BirthdayMessage = row.BirthdayMessage;
                ServerRolesName = row.ServerRolesName;
                ServerRolesDescription = row.ServerRolesDescription;
                AllowMultipleRoles = row.AllowMultipleRoles == 1;
                EnableLevels = row.EnableLevels == 1;
                LevelUpMessage = row.LevelUpMessage;
            }
        }
    }

    private void SaveNewServerConfig() {
        ConfigProcessor.SaveConfig(Guild.Id, WelcomeMessage, LeaveMessage, DefaultChannel.Id, BirthdayNotifications ? 1 : 0, PauseBdayNotifsTimer, BirthdayMessage, ServerRolesName, ServerRolesDescription, AllowMultipleRoles ? 1 : 0, EnableLevels ? 1 : 0, LevelUpMessage);
    }

    public void UpdateConfig() {
        ConfigProcessor.UpdateConfig(Guild.Id, WelcomeMessage, LeaveMessage, DefaultChannel.Id, BirthdayNotifications ? 1 : 0, PauseBdayNotifsTimer, BirthdayMessage, ServerRolesName, ServerRolesDescription, AllowMultipleRoles ? 1 : 0, EnableLevels ? 1 : 0, LevelUpMessage);
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

    // Levels Config
    public bool EnableLevels { get; set; } = false;
    public string LevelUpMessage { get; set; } = "has levelled up!";
}

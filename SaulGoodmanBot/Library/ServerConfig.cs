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
                ServerRolesName = row.ServerRolesName;
                ServerRolesDescription = row.ServerRolesDescription;
                AllowMultipleRoles = row.AllowMultipleRoles == 1;
            }
        }
    }

    private void SaveNewServerConfig() {
        ConfigProcessor.SaveConfig(Guild.Id, WelcomeMessage, LeaveMessage, DefaultChannel.Id, BirthdayNotifications ? 1 : 0, PauseBdayNotifsTimer, ServerRolesName, ServerRolesDescription, AllowMultipleRoles ? 1 : 0);
    }

    public void UpdateConfig() {
        ConfigProcessor.UpdateConfig(Guild.Id, WelcomeMessage, LeaveMessage, DefaultChannel.Id, BirthdayNotifications ? 1 : 0, PauseBdayNotifsTimer, ServerRolesName, ServerRolesDescription, AllowMultipleRoles ? 1 : 0);
    }

    // Config Properties
    private DiscordGuild Guild { get; set; }

    // General Config
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public DiscordChannel DefaultChannel { get; set; }

    // Birthday Config
    public static DateTime DATE_ERROR { get; private set; } = DateTime.Parse("1/1/1000");
    public bool BirthdayNotifications { get; set; } = true;
    public DateTime PauseBdayNotifsTimer { get; set; } = DATE_ERROR;

    // Role Config
    public string? ServerRolesName { get; set; } = null;
    public string? ServerRolesDescription { get; set; } = null;
    public bool AllowMultipleRoles { get; set; } = false;
}

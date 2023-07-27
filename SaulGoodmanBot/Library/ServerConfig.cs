using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class ServerConfig {
    public ServerConfig(ulong guildid) {
        GuildId = guildid;
        var data = ConfigProcessor.LoadConfig(GuildId);
        if (data.Count == 0) {
            SaveNewServerConfig();
        } else {
            foreach (var row in data) {
                WelcomeMessage = row.WelcomeMessage;
                LeaveMessage = row.LeaveMessage;
                BirthdayNotifications = (row.BirthdayNotifications == 1) ? true : false;
                PauseBdayNotifsTimer = row.PauseBdayNotifsTimer;
            }
        }
    }

    private void SaveNewServerConfig() {
        ConfigProcessor.SaveConfig(GuildId, WelcomeMessage, LeaveMessage, 1, PauseBdayNotifsTimer);
    }

    public void UpdateConfig() {
        ConfigProcessor.UpdateConfig(GuildId, WelcomeMessage, LeaveMessage, (BirthdayNotifications) ? 1 : 0, PauseBdayNotifsTimer);
    }

    private ulong GuildId { get; set; }
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public bool BirthdayNotifications { get; set; } = true;
    public DateTime? PauseBdayNotifsTimer { get; set; } = null;
}

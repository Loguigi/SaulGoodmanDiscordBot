namespace DataLibrary.Models;

public class ConfigModel {
    public ulong GuildId { get; set; }
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public int BirthdayNotifications { get; set; } = 1;
    public DateTime PauseBdayNotifsTimer { get; set; }
}
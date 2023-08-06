namespace DataLibrary.Models;

public class ConfigModel {
    public long GuildId { get; set; }
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public long DefaultChannel { get; set; }
    public int BirthdayNotifications { get; set; } = 1;
    public DateTime PauseBdayNotifsTimer { get; set; }
    public string? ServerRolesName { get; set; } = null;
    public string? ServerRolesDescription { get; set; } = null;
    public int AllowMultipleRoles { get; set; } = 0;
}
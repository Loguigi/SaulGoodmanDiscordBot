using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class ConfigModel {
    public long GuildId { get; set; }
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public long DefaultChannel { get; set; }
    public int BirthdayNotifications { get; set; } = 1;
    public TimeOnly BirthdayTimer { get; set; } = new TimeOnly(5, 0);
    public string BirthdayMessage { get; set; } = "Happy Birthday!";
    public string? ServerRolesName { get; set; } = null;
    public string? ServerRolesDescription { get; set; } = null;
    public int AllowMultipleRoles { get; set; } = 0;
    public int SendRoleMenuOnMemberJoin { get; set; } = 0;
    public int EnableLevels { get; set; } = 0;
    public string LevelUpMessage { get; set; } = "has levelled up!";
}
using SaulGoodmanBot.Library;

namespace SaulGoodmanLibrary.Models;

public class ConfigModel 
{
    public long GuildId { get; set; }
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public long DefaultChannel { get; set; }
    public int BirthdayNotifications { get; set; } = 1;
    public int BirthdayTimerHour { get; set; } = 0;
    public string BirthdayMessage { get; set; } = "Happy Birthday!";
    public string? ServerRolesName { get; set; } = null;
    public string? ServerRolesDescription { get; set; } = null;
    public int AllowMultipleRoles { get; set; } = 0;
    public int SendRoleMenuOnMemberJoin { get; set; } = 0;
    public int EnableLevels { get; set; } = 0;
    public string LevelUpMessage { get; set; } = "has levelled up!";
    
    public ConfigModel() { }

    public ConfigModel(ServerConfig config)
    {
        GuildId = (long)config.Guild.Id;
        WelcomeMessage = config.WelcomeMessage;
        LeaveMessage = config.LeaveMessage;
        DefaultChannel = (long)config.DefaultChannel.Id;
        BirthdayNotifications = config.BirthdayNotifications ? 1 : 0;
        BirthdayTimerHour = config.BirthdayTimerHour;
        BirthdayMessage = config.BirthdayMessage;
        ServerRolesName = config.ServerRolesName;
        ServerRolesDescription = config.ServerRolesDescription;
        AllowMultipleRoles = config.AllowMultipleRoles ? 1 : 0;
        SendRoleMenuOnMemberJoin = config.SendRoleMenuOnMemberJoin ? 1 : 0;
        EnableLevels = config.EnableLevels ? 1 : 0;
        LevelUpMessage = config.LevelUpMessage;
    }
}
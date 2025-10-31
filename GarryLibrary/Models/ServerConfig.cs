using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;

namespace GarryLibrary.Models;

public class ServerConfig
{
    public int GuildId { get; set; }
    public DiscordGuild? Guild { get; set; }
    
    #region General Config
    public string? WelcomeMessage { get; set; }
    public string? LeaveMessage { get; set; }
    public long? DefaultChannelId { get; set; }
    [NotMapped] public DiscordChannel? DefaultChannel { get; set; }
    #endregion

    #region Birthday Config
    public bool BirthdayNotifications { get; set; } = true;
    public string BirthdayMessage { get; set; } = "Happy Birthday!";
    public int BirthdayTimerHour { get; set; } = 0;
    #endregion

    #region Role Config
    public string? ServerRolesName { get; set; }
    public string? ServerRolesDescription { get; set; }
    public bool AllowMultipleRoles { get; set; }
    public bool SendRoleMenuOnMemberJoin { get; set; }
    #endregion

    #region Levels Config
    public bool EnableLevels { get; set; }
    public string LevelUpMessage { get; set; } = "has levelled up!";
    #endregion
}
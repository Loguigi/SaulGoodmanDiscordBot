using System.Data;
using System.Reflection;
using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class ServerConfig : DbBase<ConfigModel, ServerConfig> {
    #region Properties
    private DiscordGuild Guild { get; set; }

    #region General Config
    public string? WelcomeMessage { get; set; } = null;
    public string? LeaveMessage { get; set; } = null;
    public DiscordChannel DefaultChannel { get; set; }
    #endregion

    #region Birthday Config
    public static DateTime DATE_ERROR { get; private set; } = DateTime.Parse("1/1/1800");
    public bool BirthdayNotifications { get; set; } = true;
    public string BirthdayMessage { get; set; } = "Happy Birthday!";
    public DateTime PauseBdayNotifsTimer { get; set; } = DATE_ERROR;
    #endregion

    #region Role Config
    public string? ServerRolesName { get; set; } = null;
    public string? ServerRolesDescription { get; set; } = null;
    public bool AllowMultipleRoles { get; set; } = false;
    public bool SendRoleMenuOnMemberJoin { get; set; } = false;
    #endregion

    #region Levels Config
    public bool EnableLevels { get; set; } = false;
    public string LevelUpMessage { get; set; } = "has levelled up!";
    #endregion
    #endregion

    #region Public Methods
    public ServerConfig(DiscordGuild guild) {
        Guild = guild;
        DefaultChannel = Guild.GetDefaultChannel();
        
        try {
            var result = GetData("Config_GetData", new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            MapData(result.Result!);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Save() {
        try {
            var result = SaveData("Config_Process", new DynamicParameters(
                new ConfigModel() {
                    GuildId = (long)Guild.Id,
                    WelcomeMessage = WelcomeMessage,
                    LeaveMessage = LeaveMessage,
                    DefaultChannel = (long)DefaultChannel.Id,
                    BirthdayNotifications = BirthdayNotifications ? 1 : 0,
                    PauseBdayNotifsTimer = PauseBdayNotifsTimer,
                    BirthdayMessage = BirthdayMessage,
                    ServerRolesName = ServerRolesName,
                    ServerRolesDescription = ServerRolesDescription,
                    AllowMultipleRoles = AllowMultipleRoles ? 1 : 0,
                    SendRoleMenuOnMemberJoin = SendRoleMenuOnMemberJoin ? 1 : 0,
                    EnableLevels = EnableLevels ? 1 : 0,
                    LevelUpMessage = LevelUpMessage
            })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override List<ServerConfig> MapData(List<ConfigModel> data)
    {
        var config = data.First();

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

        return new List<ServerConfig>();
    }
    #endregion
}

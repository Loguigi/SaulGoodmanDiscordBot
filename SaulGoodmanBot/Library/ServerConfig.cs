using System.Data;
using System.Reflection;
using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class ServerConfig : DbBase<ConfigModel, ServerConfig> {
    public ServerConfig(DiscordGuild guild) {
        Guild = guild;
        DefaultChannel = Guild.GetDefaultChannel();
        
        try {
            var result = GetData("");
            if (result.Status != ResultArgs<List<ConfigModel>>.StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            MapData(result.Result);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Save() {
        try {
            var result = SaveData("", new ConfigModel() {
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
            });
            if (result.Status != ResultArgs<int>.StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    #region DB Methods
    protected override ResultArgs<List<ConfigModel>> GetData(string sp)
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + " @GuildId, @Status, @ErrMsg";
            var param = new ConfigModel() { GuildId = (long)Guild.Id };
            var data = cnn.Query<ConfigModel>(sql, param).ToList();

            return new ResultArgs<List<ConfigModel>>(data, param.Status, param.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override ResultArgs<int> SaveData(string sp, ConfigModel data)
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + @" @GuildId,
                @WelcomeMessage,
                @LeaveMessage
                @DefaultChannel,
                @BirthdayNotifications,
                @BirthdayMessage,
                @PauseBdayNotifsTimer,
                @ServerRolesName,
                @ServerRolesDescription,
                @AllowMultipleRoles,
                @SendRoleMenuOnMemberJoin,
                @EnableLevels,
                @LevelUpMessage,
                @Status,
                @ErrMsg";
            var result = cnn.Execute(sql, data);

            return new ResultArgs<int>(result, data.Status, data.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
        
    }

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

    #region Properties
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
    #endregion
}

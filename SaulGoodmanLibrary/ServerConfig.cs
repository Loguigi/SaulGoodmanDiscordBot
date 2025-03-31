using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class ServerConfig : DataAccess
{
    #region Properties
    public DiscordGuild Guild { get; private set; }

    #region General Config
    public string? WelcomeMessage { get; set; }
    public string? LeaveMessage { get; set; }
    public DiscordChannel DefaultChannel { get; set; }
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
    #endregion

    #region Public Methods
    public ServerConfig(DiscordGuild guild) 
    {
        Guild = guild;
        Load();
    }

    public sealed override async void Load()
    {
        Guild = await DiscordHelper.Client.GetGuildAsync(Guild.Id);
        var config = GetData<ConfigModel>(StoredProcedures.GET_CONFIG_DATA, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result.FirstOrDefault();

        if (config == null)
        {
            DefaultChannel = Guild.GetDefaultChannel();
            await Save();
            return;
        }
        
        WelcomeMessage = config.WelcomeMessage;
        LeaveMessage = config.LeaveMessage;
        DefaultChannel = Guild.GetChannel((ulong)config.DefaultChannel); 
        BirthdayNotifications = config.BirthdayNotifications == 1;
        BirthdayTimerHour = config.BirthdayTimerHour;
        BirthdayMessage = config.BirthdayMessage;
        ServerRolesName = config.ServerRolesName;
        ServerRolesDescription = config.ServerRolesDescription;
        AllowMultipleRoles = config.AllowMultipleRoles == 1;
        SendRoleMenuOnMemberJoin = config.SendRoleMenuOnMemberJoin == 1;
        EnableLevels = config.EnableLevels == 1;
        LevelUpMessage = config.LevelUpMessage;
    }

    public async Task Save()
    {
        await SaveData(StoredProcedures.PROCESS_CONFIG_DATA, new DynamicParameters(new ConfigModel(this)));
        Load();
    }
    #endregion
}

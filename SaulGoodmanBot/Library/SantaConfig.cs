using System.Data;
using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class SantaConfig : DataAccess 
{
    #region Properties
    public DiscordGuild Guild { get; }
    public DiscordRole SantaRole { get; set; }
    public bool HasStarted => ParticipationDeadline < DateTime.Now;
    public DateTime ParticipationDeadline { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string ExchangeLocation { get; set; }
    public string? ExchangeAddress { get; set;  }
    public double? PriceLimit { get; set; }
    public bool LockedIn { get; set; }
    #endregion

    #region Public Methods

    public SantaConfig(DiscordGuild guild)
    {
        Guild = guild;
        var config = GetData<SantaConfigModel>(StoredProcedures.SANTA_GET_CONFIG, new DynamicParameters(new { GuildId = (long)guild.Id })).Result.First();
        
        SantaRole = Guild.GetRole((ulong)config.SantaRoleId);
        ParticipationDeadline = config.ParticipationDeadline;
        ExchangeDate = config.ExchangeDate;
        ExchangeLocation = config.ExchangeLocation;
        ExchangeAddress = config.ExchangeAddress;
        PriceLimit = config.PriceLimit;
        LockedIn = config.LockedIn == 1;
    }
    
    public async void Update()
    {
        await SaveData(StoredProcedures.SANTA_SAVE_CONFIG, new DynamicParameters(new SantaConfigModel(this)));
    }
    #endregion
}
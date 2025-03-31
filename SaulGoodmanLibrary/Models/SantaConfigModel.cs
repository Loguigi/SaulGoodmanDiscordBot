using SaulGoodmanLibrary;

namespace SaulGoodmanLibrary.Models;

public class SantaConfigModel 
{
    public long GuildId { get; set; }
    public long SantaRoleId { get; set; }
    public DateTime ParticipationDeadline { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string ExchangeLocation { get; set; } = string.Empty;
    public string? ExchangeAddress { get; set; } = null;
    public double? PriceLimit { get; set; } = null;
    public int LockedIn { get; set; } = 0;
    
    public SantaConfigModel() {}

    public SantaConfigModel(Santa.SantaConfig config)
    {
        GuildId = (long)config.Guild.Id;
        SantaRoleId = (long)config.SantaRole.Id;
        ParticipationDeadline = config.ParticipationDeadline;
        ExchangeDate = config.ExchangeDate;
        ExchangeLocation = config.ExchangeLocation;
        ExchangeAddress = config.ExchangeAddress;
        PriceLimit = config.PriceLimit;
        LockedIn = config.LockedIn ? 1 : 0;
    }
}
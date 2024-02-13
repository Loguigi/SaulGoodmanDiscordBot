using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class SantaConfigModel {
    public long GuildId { get; set; }
    public long SantaRoleId { get; set; }
    public DateTime ParticipationDeadline { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string ExchangeLocation { get; set; } = string.Empty;
    public string? ExchangeAddress { get; set; } = null;
    public double? PriceLimit { get; set; } = null;
    public int LockedIn { get; set; } = 0;
}
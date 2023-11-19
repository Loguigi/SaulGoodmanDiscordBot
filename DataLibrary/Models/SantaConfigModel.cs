namespace DataLibrary.Models;

public class SantaConfigModel {
    public long GuildId { get; set; }
    public DateTime ParticipationDeadline { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string ExchangeLocation { get; set; } = string.Empty;
    public double? PriceLimit { get; set; } = null;
    public int LockedIn { get; set; } = 0;
}
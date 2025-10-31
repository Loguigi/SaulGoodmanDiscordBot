using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;

namespace GarryLibrary.Models;

public class SantaConfig
{
    public long GuildId { get; set; }
    public long SantaRoleId { get; set; }
    [NotMapped] public DiscordRole? SantaRole { get; set; }
    [NotMapped] public bool HasStarted => ParticipationDeadline > DateTime.Now || LockedIn;
    public DateTime ParticipationDeadline { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string ExchangeLocation { get; set; } = string.Empty;
    public string? ExchangeAddress { get; set;  }
    public double? PriceLimit { get; set; } = 0;
    public bool LockedIn { get; set; }
}
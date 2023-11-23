using DataLibrary.Logic;
using DataLibrary.Models;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library.SecretSanta;

public class SantaConfig {
    public SantaConfig(DiscordGuild guild) {
        Guild = guild;
        var config = SecretSantaProcessor.LoadConfig(Guild.Id);

        if (config == null)
            HasStarted = false;
        else {
            ParticipationDeadline = config.ParticipationDeadline;
            ExchangeDate = config.ExchangeDate;
            ExchangeLocation = config.ExchangeLocation;
            PriceLimit = config.PriceLimit;
            LockedIn = config.LockedIn == 1;
        }
    }

    public void Update() {
        if (HasStarted) {
            SecretSantaProcessor.UpdateConfig(new SantaConfigModel() {
                GuildId = (long)Guild.Id,
                ParticipationDeadline = ParticipationDeadline,
                ExchangeDate = ExchangeDate,
                ExchangeLocation = ExchangeLocation,
                PriceLimit = PriceLimit,
                LockedIn = LockedIn ? 1 : 0
            });
        } else {
            SecretSantaProcessor.StartEvent(new SantaConfigModel() {
                GuildId = (long)Guild.Id,
                ParticipationDeadline = ParticipationDeadline,
                ExchangeDate = ExchangeDate,
                ExchangeLocation = ExchangeLocation,
                PriceLimit = PriceLimit,
                LockedIn = LockedIn ? 1 : 0
            });
            HasStarted = true;
        }
    }

    private DiscordGuild Guild { get; set; }
    public bool HasStarted { get; private set; } = true;
    public DateTime ParticipationDeadline { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string ExchangeLocation { get; set; } = string.Empty;
    public double? PriceLimit { get; set; } = null;
    public bool LockedIn { get; set; } = false;
}
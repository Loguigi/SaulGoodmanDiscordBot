using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class SantaParticipant(DiscordUser user, DiscordGuild guild, string firstName, DiscordUser? giftee = null, DiscordUser? so = null, bool giftReady = false) : DataAccess
{
    #region Properties
    public DiscordUser User { get; } = user;
    public DiscordGuild Guild { get; } = guild;
    public string FirstName { get; set; } = firstName;
    public DiscordUser? Giftee { get; set; } = giftee;
    public DiscordUser? SO { get; set; } = so;
    public bool GiftReady { get; set; } = giftReady;
    #endregion
}
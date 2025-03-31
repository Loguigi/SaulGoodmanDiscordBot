using SaulGoodmanLibrary;

namespace SaulGoodmanLibrary.Models;

public class SantaParticipantModel 
{
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public long? GifteeId { get; set; }
    public long? SOId { get; set; }
    public int GiftReady { get; set; }
    
    public SantaParticipantModel() {}

    public SantaParticipantModel(ulong guildId, Santa.SantaParticipant santaParticipant)
    {
        GuildId = (long)guildId;
        UserId = (long)santaParticipant.User.Id;
        FirstName = santaParticipant.FirstName;
        GifteeId = santaParticipant.Giftee is null ? null : (long)santaParticipant.Giftee.Id;
        SOId = santaParticipant.SO is null ? null : (long)santaParticipant.SO.Id;
        GiftReady = santaParticipant.GiftReady ? 1 : 0;
    }
}
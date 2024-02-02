using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class SantaParticipantModel : DbCommonParams {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public long? GifteeId { get; set; }
    public long? SOId { get; set; }
    public int GiftReady { get; set; } = 0;
}
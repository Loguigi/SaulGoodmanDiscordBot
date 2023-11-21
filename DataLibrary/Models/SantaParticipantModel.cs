namespace DataLibrary.Models;

public class SantaParticipantModel {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public long? GifteeId { get; set; }
    public long? SOId { get; set; }
}
namespace GarryLibrary.Models;

public class SantaParticipant
{
    public int ServerMemberId { get; set; }
    public int? GifteeId { get; set; }
    public bool GiftReady { get; set; }
    public int? SignificantOtherId { get; set; }

    public ServerMember ServerMember { get; set; } = null!;
    public ServerMember? Giftee { get; set; }
    public ServerMember? SignificantOther { get; set; }
}
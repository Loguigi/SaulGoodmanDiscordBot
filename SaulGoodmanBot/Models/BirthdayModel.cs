using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class BirthdayModel {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public DateTime Birthday { get; set; }
    public int Mode { get; set; } = 0;
}
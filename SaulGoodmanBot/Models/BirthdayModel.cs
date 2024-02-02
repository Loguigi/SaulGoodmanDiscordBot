using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class BirthdayModel : DbCommonParams {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public DateTime Birthday { get; set; }
}
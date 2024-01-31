using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.DTO;

public class BirthdayDTO : DbCommonParams {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public DateTime Birthday { get; set; }
}
using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.DTO;

public class LevelDTO : DbCommonParams {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public int Level { get; set; } = 0;
    public int Experience { get; set; } = -1;
    public DateTime MsgLastSent { get; set; }
}
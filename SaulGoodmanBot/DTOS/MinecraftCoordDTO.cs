using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.DTO;

public class MinecraftWaypointDTO : DbCommonParams {
    public long GuildId { get; set; }
    public string Dimension { get; set; } = "overworld";
    public string Name { get; set; } = string.Empty;
    public int XCord { get; set; } = 0;
    public int YCord { get; set; } = 0;
    public int ZCord { get; set; } = 0;
}

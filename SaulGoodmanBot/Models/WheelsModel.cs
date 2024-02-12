using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class WheelsModel : DbCommonParams {
    public long GuildId { get; set; }
    public string WheelName { get; set; } = string.Empty;
    public string WheelOption { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = null;
    public int TempRemoved { get; set; } = 0;
}
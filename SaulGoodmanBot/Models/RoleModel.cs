using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class RoleModel {
    public long GuildId { get; set; } = 0;
    public long RoleId { get; set; } = 0;
    public string? Description { get; set; } = null;
    public string? RoleEmoji { get; set; } = null;
    public int Mode { get; set; } = 0;
}

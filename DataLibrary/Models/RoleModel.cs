using System.Numerics;

namespace DataLibrary.Models;

public class RoleModel {
    public long GuildId { get; set; } = 0;
    public long RoleId { get; set; } = 0;
    public string? Description { get; set; } = null;
    public string? RoleEmoji { get; set; } = null;
}

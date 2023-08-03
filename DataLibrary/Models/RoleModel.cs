namespace DataLibrary.Models;

public class RoleModel {
    public ulong GuildId { get; set; } = 0;
    public ulong RoleId { get; set; } = 0;
    public string? Description { get; set; } = null;
    public string? Emoji { get; set; } = null;
}

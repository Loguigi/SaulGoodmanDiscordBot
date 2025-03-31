using DSharpPlus.Entities;

namespace SaulGoodmanLibrary.Models;

public class RoleModel 
{
    public long GuildId { get; set; } = 0;
    public long RoleId { get; set; } = 0;
    public string? Description { get; set; } = null;
    public string? RoleEmoji { get; set; } = null;
    
    public RoleModel() { }

    public RoleModel(DiscordGuild guild, ServerRoles.RoleComponent role)
    {
        GuildId = (long)guild.Id;
        RoleId = (long)role.Role.Id;
        Description = role.Description == string.Empty ? null : role.Description;
        RoleEmoji = role.Emoji.GetDiscordName();
    }
}

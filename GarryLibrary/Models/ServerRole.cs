using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;

namespace GarryLibrary.Models;

public class ServerRole
{
    public long GuildId { get; set; }
    public long RoleId { get; set; }
    public string Description { get; set; }
    public string Emoji { get; set; }

    [NotMapped] public DiscordRole DiscordRole { get; set; } = null!;
}
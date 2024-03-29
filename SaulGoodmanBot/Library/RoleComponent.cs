using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class RoleComponent(DiscordRole role, string desc, DiscordEmoji emoji)
{
    public DiscordRole Role { get; set; } = role;
    public string Description { get; set; } = desc;
    public DiscordEmoji Emoji { get; set; } = emoji;
}
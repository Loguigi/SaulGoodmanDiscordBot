using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library.Roles;

public class RoleComponent {
    public RoleComponent(DiscordRole role, string desc, DiscordEmoji emoji) {
        Role = role;
        Description = desc;
        Emoji = emoji;
    }

    public DiscordRole Role { get; set; }
    public string Description { get; set; }
    public DiscordEmoji Emoji { get; set; }
}
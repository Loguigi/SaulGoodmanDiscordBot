using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class RoleComponent {
    public DiscordRole Role { get; set; }
    public string Description { get; set; }
    public DiscordEmoji Emoji { get; set; }
    
    public RoleComponent(DiscordRole role, string desc, DiscordEmoji emoji) {
        Role = role;
        Description = desc;
        Emoji = emoji;
    }
}
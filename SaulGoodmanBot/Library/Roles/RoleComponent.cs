using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library.Roles;

public class RoleComponent {

    public RoleComponent(DiscordRole role, string? desc=null, DiscordEmoji? emoji=null) {
        Role = role;
        Description = desc;
        Emoji = emoji;
    }

    public DiscordRole Role { get; private set; }
    public string? Description { get; private set; }
    public DiscordEmoji? Emoji { get; private set; }
}
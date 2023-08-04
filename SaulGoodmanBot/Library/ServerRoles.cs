using DSharpPlus.Entities;
using DataLibrary.Logic;
using DSharpPlus;

namespace SaulGoodmanBot.Library;

public class ServerRoles {
    public ServerRoles(DiscordGuild guild, DiscordClient client) {
        Guild = guild;
        Client = client;

        // assign from roles
        var data = RoleProcessor.LoadRoles(Guild.Id);
        foreach (var row in data) {
            Roles.Add(new RoleComponent(Guild.GetRole(row.RoleId), row.Description, row.Emoji != null ? DiscordEmoji.FromName(Client, row.Emoji, true) : null));
        }

        // assign from config
        var config = new ServerConfig(Guild);
        CategoryName = config.ServerRolesName ?? string.Empty;
        CategoryDescription = config.ServerRolesDescription ?? string.Empty;
        AllowMultipleRoles = config.AllowMultipleRoles;
    }

    public void Add(RoleComponent role) {
        RoleProcessor.SaveRole(Guild.Id, role.Role.Id, role.Description, role.Emoji?.ToString());
    }

    public void Remove(ulong roleid) {
        RoleProcessor.DeleteRole(Guild.Id, roleid);
    }

    public bool IsNotSetup() {
        return Roles.Count == 0;
    }

    private DiscordGuild Guild { get; set; }
    private DiscordClient Client { get; set; }
    public string CategoryName { get; private set; }
    public string CategoryDescription { get; private set; }
    public bool AllowMultipleRoles { get; private set; }
    public List<RoleComponent> Roles { get; private set; } = new();
}

public class RoleComponent {
    public RoleComponent(DiscordRole role, string? desc, DiscordEmoji? emoji) {
        Role = role;
        Description = desc;
        Emoji = emoji;
    }

    public DiscordRole Role { get; private set; }
    public string? Description { get; private set; }
    public DiscordEmoji? Emoji { get; private set; }
}

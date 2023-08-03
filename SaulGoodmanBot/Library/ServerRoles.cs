using DSharpPlus.Entities;
using DataLibrary.Logic;
using DSharpPlus;

namespace SaulGoodmanBot.Library;

public class ServerRoles {
    public ServerRoles(DiscordGuild guild) {
        Guild = guild;

        var data = RoleProcessor.LoadRoles(Guild.Id);
        foreach (var row in data) {
            Roles.Add(Guild.GetRole(row.GuildId));
        }
    }

    public void AddRoles(List<DiscordUser> roles) {
        foreach (var role in roles) {
            RoleProcessor.SaveRole(Guild.Id, role.Id);
        }
    }

    public bool IsNotSetup() {
        return Roles.Count == 0;
    }

    private DiscordGuild Guild { get; set; }
    public List<DiscordRole> Roles { get; private set; } = new List<DiscordRole>();
}

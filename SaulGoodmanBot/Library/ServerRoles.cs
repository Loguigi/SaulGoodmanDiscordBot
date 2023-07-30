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

    private DiscordGuild Guild { get; set; }
    public List<DiscordRole> Roles { get; private set; } = new List<DiscordRole>();
}

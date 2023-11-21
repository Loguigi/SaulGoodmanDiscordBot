using DSharpPlus.Entities;
using DataLibrary.Logic;
using DSharpPlus;

namespace SaulGoodmanBot.Library.Roles;

public class ServerRoles {
    public ServerRoles(DiscordGuild guild, DiscordClient client) {
        Guild = guild;
        Client = client;

        // assign from roles
        var data = RoleProcessor.LoadRoles(Guild.Id);
        try {
            foreach (var row in data) {
                Roles.Add(new RoleComponent(Guild.GetRole((ulong)row.RoleId), row.Description, row.RoleEmoji != null ? DiscordEmoji.FromName(Client, row.RoleEmoji, true) : null));
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }

        // assign from config
        var config = new ServerConfig(Guild);
        CategoryName = config.ServerRolesName ?? string.Empty;
        CategoryDescription = config.ServerRolesDescription ?? string.Empty;
        AllowMultipleRoles = config.AllowMultipleRoles;
    }

    public void Add(RoleComponent role) {
        RoleProcessor.SaveRole(Guild.Id, role.Role.Id, role.Description, role.Emoji?.GetDiscordName());
    }

    public void Remove(ulong roleid) {
        RoleProcessor.DeleteRole(Guild.Id, roleid);
    }

    public RoleComponent GetRole(ulong roleid) {
        return Roles.Where(x => x.Role.Id == roleid).First();
    }

    public bool IsNotSetup() {
        return CategoryName == string.Empty;
    }

    public bool IsEmpty() {
        return Roles.Count == 0;
    }

    public bool AlreadyExists(DiscordRole role) {
        return Roles.Contains(new RoleComponent(role));
    }

    private DiscordGuild Guild { get; set; }
    private DiscordClient Client { get; set; }
    public string CategoryName { get; private set; }
    public string CategoryDescription { get; private set; }
    public bool AllowMultipleRoles { get; private set; }
    public List<RoleComponent> Roles { get; private set; } = new();
}
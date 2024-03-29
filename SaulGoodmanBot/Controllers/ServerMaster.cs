using DSharpPlus;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Controllers;

public class ServerMaster {
    public DiscordGuild Guild { get; private set; }
    public ServerConfig Config { get; private set; }
    public WheelPickers Wheels { get; private set; }
    public ServerBirthdays Birthdays { get; private set; }
    public ServerRoles Roles { get; private set; }

    public ServerMaster(DiscordClient client, DiscordGuild guild) {
        _client = client;
        Guild = guild;
        Config = new ServerConfig(Guild);
        Wheels = new WheelPickers(Guild);
        Birthdays = new ServerBirthdays(_client, Guild);
        Roles = new ServerRoles(Guild, _client);
    }

    public async Task Refresh() {
        Config = new ServerConfig(Guild);
        Wheels = new WheelPickers(Guild);
        Birthdays = new ServerBirthdays(_client, Guild);
        Roles = new ServerRoles(Guild, _client);
        
        await Task.CompletedTask;
    }

    private DiscordClient _client;
}
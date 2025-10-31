using DSharpPlus;
using DSharpPlus.EventArgs;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class GuildEventHandler(
    ServerConfigManager configManager,
    ILogger<GuildEventHandler> logger)
    : IEventHandler<ScheduledGuildEventCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient s, ScheduledGuildEventCreatedEventArgs e)
    {
        var config = await configManager.GetConfig(e.Guild);
        await config.DefaultChannel!.SendMessageAsync($"https://discord.com/events/{e.Guild.Id}/{e.Event.Id}");
    }
}
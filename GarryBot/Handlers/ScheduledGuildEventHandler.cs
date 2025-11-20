using DSharpPlus;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class ScheduledGuildEventHandler(
    ServerConfigManager configManager,
    ILogger<ScheduledGuildEventHandler> logger)
    : IEventHandler<ScheduledGuildEventCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient s, ScheduledGuildEventCreatedEventArgs e)
    {
        var config = await configManager.GetConfig(e.Guild);
        logger.LogInformation("Scheduled event created: {EventName} in guild {Guild}", e.Event.Name, e.Guild.Name);
        var message = $"🗓️ New event created by {e.Creator.Mention}";
        await config.DefaultChannel!.SendMessageAsync(MessageTemplates.CreateGuildEventMessage(message, 
            $"https://discord.com/events/{e.Guild.Id}/{e.Event.Id}"));
    }
}
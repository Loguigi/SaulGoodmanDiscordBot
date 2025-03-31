using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SaulGoodmanBot.Handlers;

public static class GuildEventHandler {
    public static async Task HandleGuildEventCreate(DiscordClient s, ScheduledGuildEventCreateEventArgs e) 
    {
        // TODO if Config
        
        _ = await new DiscordMessageBuilder()
            .WithContent($"https://discord.com/events/{e.Guild.Id}/{e.Event.Id}")
            .SendAsync(Bot.ServerConfig[e.Guild].DefaultChannel);
    }
}
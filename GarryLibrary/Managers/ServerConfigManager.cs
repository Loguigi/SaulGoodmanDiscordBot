using DSharpPlus.Entities;
using GarryLibrary.Data;
using GarryLibrary.Models;

namespace GarryLibrary.Managers;

public class ServerConfigManager(
    IDataRepository<ServerConfig> configRepository)
{
    public async Task<ServerConfig> GetConfig(DiscordGuild guild)
    {
        var config = await configRepository.GetAsync(new { GuildId = (long)guild.Id });
        if (config is null)
        {
            var channelId = guild.GetDefaultChannel()?.Id;
            config = new ServerConfig()
            {
                DefaultChannelId = channelId == null ? null : (long)channelId,
                DefaultChannel = guild.GetDefaultChannel(),
            };
            await configRepository.CreateAsync(config);
        }
        else if (config.DefaultChannelId is not null)
        {
            config.DefaultChannel = await guild.GetChannelAsync((ulong)config.DefaultChannelId);
        }

        return config;
    }
    
    public async Task UpdateConfig(ServerConfig config)
    {
        await configRepository.UpdateAsync(config);
    }
}
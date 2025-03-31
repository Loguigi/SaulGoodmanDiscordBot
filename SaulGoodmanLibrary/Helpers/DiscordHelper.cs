using DSharpPlus;
using DSharpPlus.Entities;

namespace SaulGoodmanLibrary.Helpers;

/// <summary>
/// Singleton for Discord objects and methods
/// </summary>
public static class DiscordHelper
{
    public static DiscordClient Client { get; set; }
    public static Dictionary<DiscordGuild, ServerConfig> ServerConfigs { get; set; } = [];
    
    public static DiscordUser GetUser(ulong id) => Client.GetUserAsync(id).Result;
    public static DiscordUser GetUser(long id) => Client.GetUserAsync((ulong)id).Result;
}
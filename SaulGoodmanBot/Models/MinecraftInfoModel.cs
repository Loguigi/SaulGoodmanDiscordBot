using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Models;

public class MinecraftInfoModel {
    public long GuildId { get; set; }
    public string WorldName { get; set; } = string.Empty;
    public string? WorldDescription { get; set; } = null;
    public string? IPAddress { get; set; } = null;
    public int? MaxPlayers { get; set; } = null;
    public int Whitelist { get; set; } = 0;

    public MinecraftInfoModel() {}
    public MinecraftInfoModel(DiscordGuild guild, McConfig config) {
        GuildId = (long)guild.Id;
        WorldName = config.WorldName;
        WorldDescription = config.WorldDescription;
        IPAddress = config.IPAddress;
        MaxPlayers = config.MaxPlayers;
        Whitelist = config.Whitelist ? 1 : 0;
    }
}
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Models;

public class MinecraftWaypointModel {
    public long GuildId { get; set; }
    public string Dimension { get; set; } = "overworld";
    public string Name { get; set; } = string.Empty;
    public int XCord { get; set; } = 0;
    public int YCord { get; set; } = 0;
    public int ZCord { get; set; } = 0;

    public MinecraftWaypointModel() {}
    public MinecraftWaypointModel(DiscordGuild guild, Waypoint wp) {
        GuildId = (long)guild.Id;
        Dimension = wp.Dimension;
        Name = wp.Name;
        XCord = wp.X;
        YCord = wp.Y;
        ZCord = wp.Z;
    }
}

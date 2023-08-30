using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;

namespace SaulGoodmanBot.Library;

public class Minecraft {
    public Minecraft(DiscordGuild guild) {
        Guild = guild;
        var wps = MinecraftProcessor.LoadAllWaypoints(Guild.Id);
        var info = MinecraftProcessor.LoadMcInfo(Guild.Id).FirstOrDefault();

        if (info != null) {
            WorldName = info.WorldName;
            WorldDescription = info.WorldDescription;
            IPAddress = info.IPAddress;
            MaxPlayers = info.MaxPlayers;
            Whitelist = info.Whitelist == 1;
        } else {
            WorldName = Guild.Name + "'s Minecraft Server";
            SaveServerInfo();
        }

        foreach (var wp in wps) {
            Waypoints.Add(new Waypoint(wp.Dimension, wp.Name, wp.XCord, wp.YCord, wp.ZCord));
        }
    }

    public void SaveNewWaypoint(Waypoint wp) {
        MinecraftProcessor.SaveWaypoint(new MinecraftWaypointModel {
            GuildId = (long)Guild.Id,
            Dimension = wp.Dimension,
            Name = wp.Name,
            XCord = wp.X,
            YCord = wp.Y,
            ZCord = wp.Z
        });
    }

    public void DeleteWaypoint(Waypoint wp) {
        MinecraftProcessor.DeleteWaypoint(new MinecraftWaypointModel {
            GuildId = (long)Guild.Id,
            Dimension = wp.Dimension,
            Name = wp.Name
        });
    }

    public void SaveServerInfo() {
        MinecraftProcessor.SaveMcInfo(new MinecraftInfoModel {
            GuildId = (long)Guild.Id,
            WorldName = WorldName,
            WorldDescription = WorldDescription,
            IPAddress = IPAddress,
            MaxPlayers = MaxPlayers,
            Whitelist = Whitelist ? 1 : 0
        });
    }

    public void UpdateServerInfo() {
        MinecraftProcessor.UpdateMcInfo(new MinecraftInfoModel {
            GuildId = (long)Guild.Id,
            WorldName = WorldName,
            WorldDescription = WorldDescription,
            IPAddress = IPAddress,
            MaxPlayers = MaxPlayers,
            Whitelist = Whitelist ? 1 : 0
        });
    }

    public List<Waypoint> GetDimensionWaypoints(string dimension) {
        return Waypoints.Where(x => x.Dimension == dimension).ToList();
    }

    public bool WaypointsFull(string dimension) {
        return GetDimensionWaypoints(dimension).Count == MAX_WAYPOINTS;
    }

    public DiscordGuild Guild { get; private set; }
    public string WorldName { get; set; }
    public List<Waypoint> Waypoints { get; set; } = new();
    public string? WorldDescription { get; set; } = null;
    public string? IPAddress { get; set; } = null;
    public int? MaxPlayers { get; set; } = null;
    public bool Whitelist { get; set; } = false;
    public const int MAX_WAYPOINTS = 25;

    public class Waypoint {
        public Waypoint(string dimension, string name, int x, int y, int z) {
            Dimension = dimension;
            Name = name;
            X = x;
            Y = y;
            Z = z;
        }

        public string PrintCoords() {
            return $"{X}, {Y}, {Z}";
        }

        public string Dimension { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}

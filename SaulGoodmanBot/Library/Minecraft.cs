using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;

namespace SaulGoodmanBot.Library;

public class Minecraft {
    public Minecraft(DiscordGuild guild) {
        Guild = guild;
        var coords = MinecraftProcessor.LoadAllCoords(Guild.Id);
        var info = MinecraftProcessor.LoadMcInfo(Guild.Id).First();

        WorldName = info.WorldName;
        WorldDescription = info.WorldDescription;
        IPAddress = info.IPAddress;
        MaxPlayers = info.MaxPlayers;
        Whitelist = info.Whitelist == 1;

        foreach (var coord in coords) {
            Coords.Add(new Coord(coord.Dimension, coord.Name, coord.XCord, coord.YCord, coord.ZCord));
        }
    }

    public void SaveNewCoord(Coord coord) {
        MinecraftProcessor.SaveCoord(new MinecraftCoordModel {
            GuildId = (long)Guild.Id,
            Dimension = coord.Dimension,
            Name = coord.Name,
            XCord = coord.X,
            YCord = coord.Y,
            ZCord = coord.Z
        });
    }

    public DiscordGuild Guild { get; private set; }
    public string WorldName { get; private set; }
    public List<Coord> Coords { get; private set; } = new();
    public string? WorldDescription { get; private set; }
    public string? IPAddress { get; private set; }
    public int? MaxPlayers { get; private set; }
    public bool Whitelist { get; private set; }

    public struct Coord {
        public Coord(string dimension, string name, int x, int y, int z) {
            Dimension = dimension;
            Name = name;
            X = x;
            Y = y;
            Z = z;
        }

        public string Dimension { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}

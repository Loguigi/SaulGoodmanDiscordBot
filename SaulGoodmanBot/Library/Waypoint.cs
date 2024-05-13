using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class Waypoint {
    public string Dimension { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int Z { get; set; } = 0;
    public string Coords => $"{X}, {Y}, {Z}";
    
    public Waypoint(string dimension, string name, int x, int y, int z) {
        Dimension = dimension;
        Name = name;
        X = x;
        Y = y;
        Z = z;
    }
    public Waypoint(MinecraftWaypointModel model) {
        Dimension = model.Dimension;
        Name = model.Name;
        X = model.XCord;
        Y = model.YCord;
        Z = model.ZCord;
    }
}
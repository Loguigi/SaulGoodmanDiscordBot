namespace SaulGoodmanBot.Library;

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
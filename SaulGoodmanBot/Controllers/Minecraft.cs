using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Library;
namespace SaulGoodmanBot.Controllers;

public class Minecraft : DbBase<MinecraftWaypointModel, Waypoint> {
    public Minecraft(DiscordGuild guild) {
        Guild = guild;
        Config = new McConfig(Guild);
        var wps = MinecraftProcessor.LoadAllWaypoints(Guild.Id);
        var info = MinecraftProcessor.LoadMcInfo(Guild.Id).FirstOrDefault();

        foreach (var wp in wps) {
            Waypoints.Add(new Waypoint(wp.Dimension, wp.Name, wp.XCord, wp.YCord, wp.ZCord));
        }
    }

    public void SaveNewWaypoint(Waypoint wp) {
        Waypoints.Add(wp);
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
        Waypoints.Remove(wp);
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

    /// <summary>
    /// Queries the Waypoints list for waypoints of the chosen dimension
    /// </summary>
    /// <param name="dimension">Overworld, The Nether, or The End</param>
    /// <returns>List of waypoints for the dimension</returns>
    public List<Waypoint> GetDimensionWaypoints(string dimension) {
        return Waypoints.Where(x => x.Dimension == dimension).ToList();
    }
    
    
    public bool WaypointsFull(string dimension) {
        return GetDimensionWaypoints(dimension).Count == MAX_WAYPOINTS;
    }

    #region DB Methods
    protected override ResultArgs<List<MinecraftWaypointModel>> GetData(string sp)
    {
        throw new NotImplementedException();
    }

    protected override ResultArgs<int> SaveData(string sp, MinecraftWaypointModel data)
    {
        throw new NotImplementedException();
    }

    protected override List<Waypoint> MapData(List<MinecraftWaypointModel> data)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Properties
    public DiscordGuild Guild { get; private set; }
    public McConfig Config { get; private set; }
    public List<Waypoint> Waypoints { get; set; } = new();
    public const int MAX_WAYPOINTS = 25;
    #endregion
}

using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Library;
using System.Data;
using Dapper;
using System.Reflection;

namespace SaulGoodmanBot.Controllers;

public class Minecraft : DbBase {
    #region Properties
    public DiscordGuild Guild { get; private set; }
    public McConfig Config { get; set; } = new();
    public List<Waypoint> Waypoints { get; set; } = [];
    #endregion

    #region Public Methods
    public Minecraft(DiscordGuild guild) {
        Guild = guild;
        
        try {
            var result = GetMultipleData(StoredProcedures.MINECRAFT_GETDATA, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;
            if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    /// <summary>
    /// Queries the Waypoints list for waypoints of the chosen dimension
    /// </summary>
    /// <param name="dimension">Overworld, The Nether, or The End</param>
    /// <returns>List of waypoints for the dimension</returns>
    public List<Waypoint> GetDimensionWaypoints(string dimension) {
        return Waypoints.Where(x => x.Dimension == dimension).ToList();
    }

    public void SaveWaypoint(Waypoint wp) {
        try {
            var result = SaveData(StoredProcedures.MINECRAFT_SAVEWAYPOINT, new DynamicParameters(
                new MinecraftWaypointModel(Guild, wp)
            )).Result;
            if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void DeleteWaypoint(Waypoint wp) {
        try {
            var result = SaveData("", new DynamicParameters(
                new MinecraftWaypointModel(Guild, wp)
            )).Result;
            if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void SaveConfig() {
        try {
            var result = SaveData(StoredProcedures.MINECRAFT_SAVECONFIG, new DynamicParameters(
                new MinecraftInfoModel(Guild, Config)
            )).Result;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override async Task<ResultArgs<SqlMapper.GridReader>> GetMultipleData(string sp, DynamicParameters param)
    {
        var result = await base.GetMultipleData(sp, param);
        if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
        if (result.Result == null) return result;

        var data = result.Result!;
        Config = new McConfig(data.Read<MinecraftInfoModel>().First());

        var waypoints = data.Read<MinecraftWaypointModel>();
        foreach (var w in waypoints) {
            Waypoints.Add(new Waypoint(w));
        }

        return result;
    }
    #endregion
}

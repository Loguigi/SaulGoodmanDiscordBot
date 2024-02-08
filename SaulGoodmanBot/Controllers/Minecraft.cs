// using DSharpPlus.Entities;
// using SaulGoodmanBot.Data;
// using SaulGoodmanBot.Models;
// using SaulGoodmanBot.Library;
// using System.Data;
// using Dapper;
// namespace SaulGoodmanBot.Controllers;

// public class Minecraft : DbBase<MinecraftWaypointModel, Waypoint> {
//     #region Properties
//     public DiscordGuild Guild { get; private set; }
//     public McConfig Config { get; set; }
//     public List<Waypoint> Waypoints { get; set; } = new();
//     #endregion

//     public Minecraft(DiscordGuild guild) {
//         Guild = guild;
//         Config = new McConfig(Guild);
        
//         try {
//             var result = GetData("");
//             if (result.Status != ResultArgs<List<MinecraftWaypointModel>>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message);
//             Waypoints = MapData(result.Result);
//         } catch (Exception ex) {

//         }
//     }

//     public void SaveNewWaypoint(Waypoint wp) {
//         try {
//             var result = SaveData("", new MinecraftWaypointModel {
//                 GuildId = (long)Guild.Id,
//                 Dimension = wp.Dimension,
//                 Name = wp.Name,
//                 XCord = wp.X,
//                 YCord = wp.Y,
//                 ZCord = wp.Z,
//                 Mode = (int)DataMode.SAVE
//             });
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     public void DeleteWaypoint(Waypoint wp) {
//         try {
//             var result = SaveData("", new MinecraftWaypointModel {
//                 GuildId = (long)Guild.Id,
//                 Dimension = wp.Dimension,
//                 Name = wp.Name,
//                 XCord = wp.X,
//                 YCord = wp.Y,
//                 ZCord = wp.Z,
//                 Mode = (int)DataMode.DELETE
//             });
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     /// <summary>
//     /// Queries the Waypoints list for waypoints of the chosen dimension
//     /// </summary>
//     /// <param name="dimension">Overworld, The Nether, or The End</param>
//     /// <returns>List of waypoints for the dimension</returns>
//     public List<Waypoint> GetDimensionWaypoints(string dimension) {
//         return Waypoints.Where(x => x.Dimension == dimension).ToList();
//     }

//     #region DB Methods
//     protected override ResultArgs<List<MinecraftWaypointModel>> GetData(string sp)
//     {
//         try {
//             using IDbConnection cnn = Connection;
//             var sql = sp + " @GuildId, @Status, @ErrMsg";
//             var param = new MinecraftWaypointModel() { GuildId = (long)Guild.Id };
//             var data = cnn.Query<MinecraftWaypointModel>(sql, param).ToList();

//             return new ResultArgs<List<MinecraftWaypointModel>>(data, param.Status, param.ErrMsg);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     protected override ResultArgs<int> SaveData(string sp, MinecraftWaypointModel data)
//     {
//         try {
//             using IDbConnection cnn = Connection;
//             var sql = sp + @" @GuildId,
//                 @Dimension,
//                 @Name,
//                 @XCord,
//                 @YCord,
//                 @ZCord,
//                 @Mode,
//                 @Status,
//                 @ErrMsg";
//             var result = cnn.Execute(sql, data);
            
//             return new ResultArgs<int>(result, data.Status, data.ErrMsg);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     protected override List<Waypoint> MapData(List<MinecraftWaypointModel> data)
//     {
//         var waypoints = new List<Waypoint>();
//         foreach (var w in data) {
//             waypoints.Add(new Waypoint() {
//                 Dimension = w.Dimension,
//                 Name = w.Name,
//                 X = w.XCord,
//                 Y = w.YCord,
//                 Z = w.ZCord
//             });
//         }

//         return waypoints;
//     }

//     private enum DataMode {
//         SAVE = 0,
//         DELETE = 1
//     }
//     #endregion
// }

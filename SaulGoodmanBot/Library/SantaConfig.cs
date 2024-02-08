// using System.Data;
// using Dapper;
// using DSharpPlus.Entities;
// using SaulGoodmanBot.Data;
// using SaulGoodmanBot.Models;

// namespace SaulGoodmanBot.Library;

// public class SantaConfig : DbBase<SantaConfigModel, SantaConfig> {
//     #region Properties
//     private DiscordGuild Guild { get; set; }
//     public DiscordRole SantaRole { get; set; }
//     public bool HasStarted { get; private set; } = true;
//     public DateTime ParticipationDeadline { get; set; }
//     public DateTime ExchangeDate { get; set; }
//     public string ExchangeLocation { get; set; } = string.Empty;
//     public string? ExchangeAddress { get; set; } = null;
//     public double? PriceLimit { get; set; } = null;
//     public bool LockedIn { get; set; } = false;
//     #endregion

//     public SantaConfig(DiscordGuild guild) {
//         Guild = guild;
//         try {
//             var result = GetData("");
//             if (result.Status != ResultArgs<List<SantaConfigModel>>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message);
//             MapData(result.Result);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     public void Update() {
//         try {
//             var result = SaveData("", new SantaConfigModel() {
//                 GuildId = (long)Guild.Id,
//                 SantaRoleId = (long)SantaRole.Id,
//                 ParticipationDeadline = ParticipationDeadline,
//                 ExchangeDate = ExchangeDate,
//                 ExchangeLocation = ExchangeLocation,
//                 ExchangeAddress = ExchangeAddress,
//                 PriceLimit = PriceLimit,
//                 LockedIn = LockedIn ? 1 : 0
//             });

//             if (result.Status != ResultArgs<int>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message);
//         } catch (Exception ex) {

//         }
//     }

//     #region DB Methods
//     protected override ResultArgs<List<SantaConfigModel>> GetData(string sp)
//     {
//         try {
//             using IDbConnection cnn = Connection;
//             var sql = sp + " @GuildId, @Status, @ErrMsg";
//             var param = new SantaConfigModel() { GuildId = (long)Guild.Id };
//             var data = cnn.Query<SantaConfigModel>(sql, param).ToList();

//             return new ResultArgs<List<SantaConfigModel>>(data, param.Status, param.ErrMsg);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     protected override ResultArgs<int> SaveData(string sp, SantaConfigModel data)
//     {
//         try {
//             using IDbConnection cnn = Connection;
//             var sql = sp + @" @GuildId,
//                 @SantaRoleId,
//                 @ParticipationDeadline,
//                 @ExchangeDate,
//                 @ExchangeLocation,
//                 @ExchangeAddress,
//                 @PriceLimit,
//                 @LockedIn,
//                 @Status,
//                 @ErrMsg";
//             var result = cnn.Execute(sql, data);
            
//             return new ResultArgs<int>(result, data.Status, data.ErrMsg);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     protected override List<SantaConfig> MapData(List<SantaConfigModel> data)
//     {
//         var config = data.FirstOrDefault();

//         if (config == null) {
//             HasStarted = false;
//             return new List<SantaConfig>();
//         }

//         SantaRole = Guild.GetRole((ulong)config.SantaRoleId);
//         ParticipationDeadline = config.ParticipationDeadline;
//         ExchangeDate = config.ExchangeDate;
//         ExchangeLocation = config.ExchangeLocation;
//         ExchangeAddress = config.ExchangeAddress;
//         PriceLimit = config.PriceLimit;
//         LockedIn = config.LockedIn == 1;

//         return new List<SantaConfig>();
//     }
//     #endregion
// }
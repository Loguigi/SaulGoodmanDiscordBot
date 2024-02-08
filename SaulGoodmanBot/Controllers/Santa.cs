// using DSharpPlus;
// using DSharpPlus.Entities;
// using SaulGoodmanBot.Data;
// using SaulGoodmanBot.Models;
// using SaulGoodmanBot.Helpers;
// using SaulGoodmanBot.Library;
// using System.Data;
// using Dapper;
// using System.Collections;

// namespace SaulGoodmanBot.Controllers;

// public class Santa : DbBase<SantaParticipantModel, SantaParticipant>, IEnumerable<SantaParticipant>, IEnumerable {
//     #region Properties
//     private DiscordClient Client { get; set; }
//     private DiscordGuild Guild { get; set; }
//     public SantaConfig Config { get; private set; }
//     public List<SantaParticipant> Participants { get; private set; } = new();
//     #endregion
    
//     #region Public Methods
//     public Santa(DiscordClient client, DiscordGuild guild) {
//         try {
//             Client = client;
//             Guild = guild;
//             Config = new SantaConfig(Guild);

//             var result = GetData("");
//             if (result.Status != ResultArgs<List<SantaParticipantModel>>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message);
//             Participants = MapData(result.Result);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     public SantaParticipant? this[DiscordUser user] {
//         get => Participants.Where(x => x.User == user).FirstOrDefault();
//     }

//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//     public IEnumerator<SantaParticipant> GetEnumerator() => Participants.GetEnumerator();

//     public void StartEvent(SantaConfig config) {
//         Config = config;
//         Config.Update();
//     }

//     public void EndEvent() {
//         SecretSantaProcessor.EndEvent(Guild.Id);
//     }

//     public void AddParticipant(SantaParticipant participant) {
//         try {
//             var result = SaveData("", new SantaParticipantModel() {
//                 GuildId = (long)Guild.Id,
//                 UserId = (long)participant.User.Id,
//                 FirstName = participant.FirstName,
//                 Mode = (int)DataMode.ADD_PARTICIPANT
//             });
//             if (result.Status != ResultArgs<int>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message);
//         } catch (Exception ex) {

//         }
//     }

//     public void AddCouple(SantaParticipant user1, SantaParticipant user2) {
//         user1.SO = user2.User;
//         user2.SO = user1.User;
        
//         try {
//             var result = SaveData("", new SantaParticipantModel() {
//                 GuildId = (long)Guild.Id,
//                 UserId = (long)user1.User.Id,
//                 SOId = (long)user1.SO.Id
//             });
//             if (result.Status != ResultArgs<int>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message);

//             result = SaveData("", new SantaParticipantModel() {
//                 GuildId = (long)Guild.Id,
//                 UserId = (long)user2.User.Id,
//                 SOId = (long)user2.SO.Id
//             });
//             if (result.Status != ResultArgs<int>.StatusCodes.SUCCESS)
//                 throw new Exception(result.Message); 
//         } catch (Exception ex) {

//         }
//     }

//     public void AssignNames() {
//         Config.LockedIn = true;
//         Config.Update();
//         var shuffledParticipants = Participants.OrderBy(x => RandomHelper.RNG.Next()).ToList();

//         foreach (var p in Participants) {
//             var y = shuffledParticipants.Where(x => x.User != p.User).ToList();
//             if (p.SO! != null!)
//                 y = shuffledParticipants.Where(x => x.User != p.User && x.User != p.SO).ToList();

//             p.Giftee = y[RandomHelper.RNG.Next(y.Count)].User;
//             var result = SaveData("", new SantaParticipantModel() {
//                 GuildId = (long)Guild.Id,
//                 UserId = (long)p.User.Id,
//                 GifteeId = (long)p.Giftee.Id,
//                 Mode = (int)DataMode.UPDATE_PARTICIPANT
//             });
//             shuffledParticipants.Remove(Participants.Where(x => x.User == p.Giftee).First());
//         }
//     }

//     public bool NotEnoughParticipants() {
//         return Participants.Count < 3;
//     }
//     #endregion

//     #region DB Methods
//     protected override ResultArgs<List<SantaParticipantModel>> GetData(string sp)
//     {
//         try {
//             using IDbConnection cnn = Connection;
//             var sql = sp + " @GuildId, @Status, @ErrMsg";
//             var param = new SantaParticipantModel() { GuildId = (long)Guild.Id };
//             var data = cnn.Query<SantaParticipantModel>(sql, param).ToList();

//             return new ResultArgs<List<SantaParticipantModel>>(data, param.Status, param.ErrMsg);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     protected override ResultArgs<int> SaveData(string sp, SantaParticipantModel data)
//     {
//         try {
//             using IDbConnection cnn = Connection;
//             var sql = sp + @" @GuildId,
//                 @UserId,
//                 @FirstName,
//                 @GifteeId,
//                 @SOId,
//                 @GiftReady,
//                 @Mode,
//                 @Status,
//                 @ErrMsg";
//             var result = cnn.Execute(sql, data);

//             return new ResultArgs<int>(result, data.Status, data.ErrMsg);
//         } catch (Exception ex) {
//             throw;
//         }
//     }

//     protected override List<SantaParticipant> MapData(List<SantaParticipantModel> data)
//     {
//         var participants = new List<SantaParticipant>();
//         foreach (var p in data) {
//             participants.Add(new SantaParticipant(GetUser(Client, (ulong)p.UserId).Result) {
//                 FirstName = p.FirstName,
//                 Giftee = p.GifteeId == null ? null : GetUser(Client, (ulong)p.GifteeId).Result,
//                 SO = p.SOId == null ? null : GetUser(Client, (ulong)p.SOId).Result,
//                 GiftReady = p.GiftReady == 1
//             });
//         }

//         return participants;
//     }

//     private enum DataMode {
//         ADD_PARTICIPANT = 0,
//         UPDATE_PARTICIPANT = 1,
//     }
//     #endregion
// }
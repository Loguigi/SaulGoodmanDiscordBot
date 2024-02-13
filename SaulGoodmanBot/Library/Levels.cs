using DSharpPlus.Entities;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using Dapper;

namespace SaulGoodmanBot.Library;

public class Levels : DbBase<LevelModel, Levels> {
    #region Properties
    private DiscordGuild Guild { get; set; }
    public DiscordUser User { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }
    public int Rank { get; private set; }
    public int ExpNeededForNextLevel { get => 2 * (int)Math.Pow(Level + 1, 2) - 2; }
    public DateTime MsgLastSent { get; private set; }
    public DateTime NewMsgSent { get; set; }
    public bool LevelledUp { get; set; } = false;
    private const int EXP_GAIN = 1;
    #endregion

    #region Public Methods
    public Levels(DiscordGuild guild, DiscordUser user) {
        Guild = guild;
        User = user;

        try {
            var result = GetData("Levels_GetData", new DynamicParameters( new { GuildId = (long)Guild.Id, UserId = (long)User.Id})).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            MapData(result.Result!);
            Rank = GetRank();
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void GrantExp() {
        try {
            Experience += EXP_GAIN;
            int newLevel = (int)Math.Sqrt((Experience/2) + 1);
            if (newLevel > Level) {
                Level++;
                LevelledUp = true;
            }
            var result = SaveData("Levels_Process", new DynamicParameters(
                new LevelModel() {
                    GuildId = (long)Guild.Id,
                    UserId = (long)User.Id,
                    Experience = Experience,
                    Level = Level,
                    MsgLastSent = NewMsgSent
            })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override List<Levels> MapData(List<LevelModel> data)
    {
        try
        {
            var level = data.FirstOrDefault();
            Experience = level?.Experience ?? -1;
            Level = level?.Level ?? 0;
            MsgLastSent = level?.MsgLastSent ?? DateTime.Now;

            return new List<Levels>();
        }
        catch (Exception ex)
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private int GetRank() {
        try {
            var param = new DynamicParameters(new { GuildId = (long)Guild.Id, UserId = (long)User.Id });
            param.Add("@Rank", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var result = SaveData("Levels_GetRank", param).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            return param.Get<int>("@Rank");
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion
}

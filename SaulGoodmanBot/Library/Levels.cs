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
            var result = GetData();
            if (result.Status != ResultArgs<List<LevelModel>>.StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            MapData(result.Result);
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
            var result = SaveData(new LevelModel() {
                GuildId = (long)Guild.Id,
                UserId = (long)User.Id,
                Experience = Experience,
                Level = Level,
                MsgLastSent = NewMsgSent
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override ResultArgs<List<LevelModel>> GetData(string sp="Levels_GetData")
    {
        try
        {
            using IDbConnection cnn = Connection;
            var sql = sp += " @GuildId, @UserId, @Status, @ErrMsg";
            var param = new LevelModel() { GuildId = (long)Guild.Id, UserId = (long)User.Id };
            var result = cnn.Query<LevelModel>(sql, param).ToList();
            return new ResultArgs<List<LevelModel>>(result, param.Status, param.ErrMsg);
        }
        catch (Exception ex)
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override ResultArgs<int> SaveData(LevelModel data, string sp="Levels_Process")
    {
        try
        {
            using IDbConnection cnn = Connection;
            var param = sp + @" @GuildId, 
                @UserId,
                @Experience,
                @Level,
                @MsgLastSent,
                @Status,
                @ErrMsg";
            var result = cnn.Execute(param, data);
            return new ResultArgs<int>(result, data.Status, data.ErrMsg);
        }
        catch (Exception ex)
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override List<Levels> MapData(List<LevelModel> data)
    {
        try
        {
            var level = data.First();
            Experience = level.Experience;
            Level = level.Level;
            MsgLastSent = level.MsgLastSent;
            Rank = level.Rank;

            return new List<Levels>();
        }
        catch (Exception ex)
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion
}

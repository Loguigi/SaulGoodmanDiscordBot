using DSharpPlus.Entities;
using DataLibrary.Logic;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace SaulGoodmanBot.Library;

internal class Levels : DbBase {
    public Levels(DiscordGuild guild, DiscordUser user) {
        Guild = guild;
        User = user;

        try {
            var result = DBGetData();
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    #region DB Methods
    private ResultArgs DBGetData() {
        try {
            #region Parameters
            SqlParameter guildId;
            SqlParameter userId;
            SqlParameter level;
            SqlParameter exp;
            SqlParameter rank;
            SqlParameter msgLastSent;
            SqlParameter status;
            SqlParameter errMsg;
            #endregion

            using SqlConnection cnn = new(ConnectionString);
            cnn.Open();
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            // TODO SP

            guildId = new SqlParameter()
            {
                ParameterName = "@p_GuildId",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Input,
                Value = Guild.Id
            };
            cmd.Parameters.Add(guildId);

            userId = new SqlParameter()
            {
                ParameterName = "@p_UserId",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Input,
                Value = User.Id
            };
            cmd.Parameters.Add(userId);

            level = new SqlParameter()
            {
                ParameterName = "@p_Level",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(level);

            exp = new SqlParameter()
            {
                ParameterName = "@p_Exp",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(exp);

            msgLastSent = new SqlParameter()
            {
                ParameterName = "@p_MsgLastSent",
                SqlDbType = SqlDbType.DateTime,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(msgLastSent);

            rank = new SqlParameter()
            {
                ParameterName = "@p_Rank",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(rank);

            status = new SqlParameter()
            {
                ParameterName = "@p_Status",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(status);

            errMsg = new SqlParameter()
            {
                ParameterName = "@p_ErrMsg",
                SqlDbType = SqlDbType.VarChar,
                Size = 500,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(errMsg);

            var da = new SqlDataAdapter() { SelectCommand = cmd };
            var result = new ResultArgs((int)status.Value, errMsg.Value.ToString() ?? throw new Exception());

            Level = DeNull(level.Value, 0);
            Experience = DeNull(exp.Value, -1);
            Rank = DeNull(rank.Value, 0);
            MsgLastSent = DeNull(msgLastSent.Value, DateTime.MinValue);

            return result;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private ResultArgs DBUpdateExp() {
        try {
            #region Parameters
            SqlParameter guildId;
            SqlParameter userId;
            SqlParameter exp;
            SqlParameter level;
            SqlParameter newMsgSent;
            SqlParameter status;
            SqlParameter errMsg;
            #endregion

            using SqlConnection cnn = new(ConnectionString);
            cnn.Open();
            var cmd = cnn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            // TODO sp

            guildId = new SqlParameter() {
                ParameterName = "@p_GuildId",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Input,
                Value = Guild.Id
            };
            cmd.Parameters.Add(guildId);

            userId = new SqlParameter() {
                ParameterName = "@p_UserId",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Input,
                Value = User.Id
            };
            cmd.Parameters.Add(userId);

            exp = new SqlParameter() {
                ParameterName = "@p_Exp",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = Experience
            };
            cmd.Parameters.Add(exp);

            level = new SqlParameter() {
                ParameterName = "@p_Level",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = Level
            };
            cmd.Parameters.Add(level);

            newMsgSent = new SqlParameter() {
                ParameterName = "@p_NewMsgSent",
                SqlDbType = SqlDbType.DateTime,
                Direction = ParameterDirection.Input,
                Value = NewMsgSent
            };
            cmd.Parameters.Add(newMsgSent);

            status = new SqlParameter() {
                ParameterName = "@p_Status",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(status);

            errMsg = new SqlParameter() {
                ParameterName = "@p_ErrMsg",
                SqlDbType = SqlDbType.VarChar,
                Size = 500,
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(errMsg);

            cmd.ExecuteNonQuery();
            return new ResultArgs((int)status.Value, errMsg.Value.ToString()!);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    public void GrantExp() {
        try {
            Experience += EXP_GAIN;
            int newLevel = (int)Math.Sqrt((Experience/2) + 1);
            if (newLevel > Level) {
                Level++;
                LevelledUp = true;
            }
            var result = DBUpdateExp();
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

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
}

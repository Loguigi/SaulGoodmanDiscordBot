using DSharpPlus.Entities;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace SaulGoodmanBot.Library.Birthdays;

internal class ServerBirthdays : DbBase, IEnumerable<Birthday> {
    public ServerBirthdays(DiscordGuild guild) {
        Guild = guild;
        
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
    private enum DataMode {
        ADD,
        CHANGE,
        REMOVE
    }

    private ResultArgs DBGetData() {
        try {
            #region Parameters
            SqlParameter guildId;
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

            var result = new ResultArgs((int)status.Value, errMsg.Value.ToString()!);
            var da = new SqlDataAdapter() { SelectCommand = cmd };
            var ds = new DataSet();
            da.Fill(ds);

            if (result.Result != 0)
                return result;

            var dtr = ds.CreateDataReader();
            if (!dtr.HasRows)
                return result;

            while (dtr.Read()) {
                Birthdays.Add(new Birthday(GetUser(DeNull<ulong>(dtr["UserId"], 0)).Result, DeNull(dtr["Birthday"], DateTime.MinValue)));
            }

            return result;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private ResultArgs DBProcess(Birthday bday, DataMode mode) {
        try {
            #region Parameters
            SqlParameter guildId;
            SqlParameter param;
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

            param = new SqlParameter() {
                ParameterName = "@p_UserId",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Input,
                Value = bday.User.Id
            };
            cmd.Parameters.Add(param);

            param = new SqlParameter() {
                ParameterName = "@p_Birthday",
                SqlDbType = SqlDbType.DateTime,
                Direction = ParameterDirection.Input,
                Value = bday.BDay
            };
            cmd.Parameters.Add(param);

            param = new SqlParameter() {
                ParameterName = "@p_Mode",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = (int)mode
            };
            cmd.Parameters.Add(param);

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

    private async Task<DiscordUser> GetUser(ulong userid) => await Guild.GetMemberAsync(userid);
    #endregion

    #region Public Methods
    public Birthday this[DiscordUser user] {
        get => Birthdays.Where(x => x.User == user).FirstOrDefault()!;
        set {
            try {
                if (this[value.User] == null)
                    Add(value);
                else
                    Change(value);
            } catch (Exception ex) {
                ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
                throw;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Birthday> GetEnumerator() => Birthdays.GetEnumerator();

    public void Add(Birthday bday) {
        try {
            var result = DBProcess(bday, DataMode.ADD);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Change(Birthday bday) {
        try {
            var result = DBProcess(bday, DataMode.CHANGE);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Remove(Birthday bday) {
        try {
            var result = DBProcess(bday, DataMode.REMOVE);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public bool IsEmpty() {
        return Birthdays.Count == 0;
    }
    #endregion

    #region Properties
    private DiscordGuild Guild { get; set; }
    public List<Birthday> Birthdays { get; private set; } = new();
    public Birthday Next {
        get {
            var nextBirthdays = Birthdays;

            // change birthday years to next birthday
            foreach (var birthday in nextBirthdays) {
                birthday.BDay = birthday.BDay.AddYears(birthday.Age + 1);
            }

            // sort to find next birthday
            nextBirthdays.Sort((d1, d2) => DateTime.Compare(d1.BDay, d2.BDay));

            return nextBirthdays.FirstOrDefault() ?? throw new Exception($"No birthdays in {Guild.Name}");
        }
    }
    #endregion
}

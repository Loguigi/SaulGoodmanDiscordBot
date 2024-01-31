using DSharpPlus.Entities;
using DataLibrary.Logic;
using DSharpPlus;
using System.Collections;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace SaulGoodmanBot.Library.Roles;

internal class ServerRoles : DbBase, IEnumerable<RoleComponent> {
    public ServerRoles(DiscordGuild guild, DiscordClient client) {
        Guild = guild;
        Client = client;

        try {
            var result = DBGetData();
            if (result.Result == ResultArgs.ResultCodes.ERROR)
                throw new Exception(result.Message);

            var config = new ServerConfig(Guild);
            CategoryName = config.ServerRolesName ?? string.Empty;
            CategoryDescription = config.ServerRolesDescription ?? string.Empty;
            AllowMultipleRoles = config.AllowMultipleRoles;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<RoleComponent> GetEnumerator() => Roles.GetEnumerator();

    public RoleComponent this[ulong roleid] {
        get => Roles.Where(x => x.Role.Id == roleid).FirstOrDefault() ?? throw new Exception("Role not found");
    }

    public RoleComponent this[DiscordRole role] {
        get => Roles.Where(x => x.Role == role).FirstOrDefault() ?? throw new Exception("Role not found");
    }

    public bool Contains(ulong roleid) => Roles.Exists(x => x.Role.Id == roleid);
    public bool Contains(DiscordRole role) => Roles.Exists(x => x.Role == role);

    public void Add(RoleComponent role) {
        try {
            var result = DBProcess(role, DataMode.ADD);
            if (result.Result != 0) {
                throw new Exception(result.Message);
            }
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Remove(RoleComponent role) {
        try {
            var result = DBProcess(role, DataMode.DELETE);
            if (result.Result != 0) {
                throw new Exception(result.Message);
            }
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public bool HasRole(DiscordMember member, RoleComponent role) => member.Roles.Contains(role.Role);

    #region DB Methods
    private enum DataMode {
        ADD,
        DELETE
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
            cmd.Parameters.Add(errMsg);

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
                Roles.Add(new RoleComponent(
                    role:Guild.GetRole(DeNull<ulong>(dtr["GuildId"], 0)),
                    desc:DeNull(dtr["Description"], string.Empty),
                    emoji:DiscordEmoji.FromName(Client, DeNull(dtr["Emoji"], ":blue_circle:"), true)
                ));
            }

            return result;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private ResultArgs DBProcess(RoleComponent role, DataMode mode) {
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
                ParameterName = "@p_RoleId",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Input,
                Value = role.Role.Id
            };
            cmd.Parameters.Add(param);

            param = new SqlParameter() {
                ParameterName = "@p_Description",
                SqlDbType = SqlDbType.VarChar,
                Size = 200,
                Direction = ParameterDirection.Input,
                Value = role.Description == string.Empty ? null : role.Description
            };
            cmd.Parameters.Add(param);

            param = new SqlParameter() {
                ParameterName = "@p_RoleEmoji",
                SqlDbType = SqlDbType.VarChar,
                Size = 100,
                Direction = ParameterDirection.Input,
                Value = role.Emoji.GetDiscordName() == ":blue_circle:" ? null : role.Emoji.GetDiscordName()
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
    #endregion

    private DiscordGuild Guild { get; }
    private DiscordClient Client { get; }
    public string CategoryName { get; private set; }
    public string CategoryDescription { get; private set; }
    public bool AllowMultipleRoles { get; private set; }
    public bool IsNotSetup { get => CategoryName == string.Empty && Roles.Count == 0; }
    public List<RoleComponent> Roles { get; private set; } = new();
    public DiscordEmoji DEFAULT_EMOJI { get => DiscordEmoji.FromName(Client, ":blue_circle:", false); }
}

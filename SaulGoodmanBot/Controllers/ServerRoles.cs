using DSharpPlus.Entities;
using DSharpPlus;
using System.Collections;
using System.Reflection;
using System.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Library;
using Dapper;

namespace SaulGoodmanBot.Controllers;

public class ServerRoles : DbBase<RoleModel, RoleComponent>, IEnumerable<RoleComponent> {
    #region Public Methods
    public ServerRoles(DiscordGuild guild, DiscordClient client) {
        Guild = guild;
        Client = client;

        try {
            var result = GetData();
            if (result.Status != ResultArgs<List<RoleModel>>.StatusCodes.SUCCESS)
                throw new Exception(result.Message);

            Roles = MapData(result.Result);
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
            var result = SaveData(new RoleModel() {
                GuildId = (long)Guild.Id,
                RoleId = (long)role.Role.Id,
                Description = role.Description == string.Empty ? null : role.Description,
                RoleEmoji = role.Emoji.GetDiscordName(),
                Mode = (int)DataMode.ADD
            });
            if (result.Status == ResultArgs<int>.StatusCodes.ERROR)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Remove(RoleComponent role) {
        try {
            var result = SaveData(new RoleModel() {
                GuildId = (long)Guild.Id,
                RoleId = (long)role.Role.Id,
                Mode = (int)DataMode.DELETE
            });
            if (result.Status == ResultArgs<int>.StatusCodes.ERROR)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public bool HasRole(DiscordMember member, RoleComponent role) => member.Roles.Contains(role.Role);
    #endregion

    #region DB Methods
    protected override ResultArgs<List<RoleModel>> GetData(string sp="Roles_GetData")
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + " @GuildId, @Status, @ErrMsg";
            var param = new RoleModel() { GuildId = (long)Guild.Id };
            var data = cnn.Query<RoleModel>(sql, param).ToList();

            return new ResultArgs<List<RoleModel>>(data, param.Status, param.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override ResultArgs<int> SaveData(RoleModel data, string sp="Roles_Process")
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + @" @GuildId,
                @RoleId,
                @Description,
                @RoleEmoji,
                @Mode,
                @Status,
                @ErrMsg";
            var result = cnn.Execute(sql, data);

            return new ResultArgs<int>(result, data.Status, data.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override List<RoleComponent> MapData(List<RoleModel> data)
    {
        var roles = new List<RoleComponent>();
        foreach (var r in data) {
            roles.Add(new RoleComponent(Guild.GetRole((ulong)r.RoleId), r.Description ?? string.Empty, DiscordEmoji.FromName(Client, r.RoleEmoji ?? DEFAULT_EMOJI.Name, true)));
        }

        return roles;
    }

    private enum DataMode {
        ADD,
        DELETE
    }
    #endregion

    #region Properties
    private DiscordGuild Guild { get; }
    private DiscordClient Client { get; }
    public string CategoryName { get; private set; }
    public string CategoryDescription { get; private set; }
    public bool AllowMultipleRoles { get; private set; }
    public bool IsNotSetup { get => CategoryName == string.Empty && Roles.Count == 0; }
    public List<RoleComponent> Roles { get; private set; } = new();
    public DiscordEmoji DEFAULT_EMOJI { get => DiscordEmoji.FromName(Client, ":blue_circle:", false); }
    #endregion
}

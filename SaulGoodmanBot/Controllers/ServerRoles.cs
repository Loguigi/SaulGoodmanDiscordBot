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
    
    #region Public Methods
    public ServerRoles(DiscordGuild guild, DiscordClient client) {
        Guild = guild;
        Client = client;

        try {
            var result = GetData("Roles_GetData", new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);

            Roles = MapData(result.Result!);
            var config = new ServerConfig(Guild);
            CategoryName = config.ServerRolesName ?? string.Empty;
            CategoryDescription = config.ServerRolesDescription ?? string.Empty;
            AllowMultipleRoles = config.AllowMultipleRoles;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public RoleComponent? this[DiscordRole role] { get => Roles.Where(x => x.Role == role).FirstOrDefault(); }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<RoleComponent> GetEnumerator() => Roles.GetEnumerator();

    public bool TryGetRole(DiscordRole role, out RoleComponent? saved_roll) {
        saved_roll = this[role];
        return saved_roll != null;
    }

    public void Add(RoleComponent role) {
        try {
            var result = SaveData("Roles_Process", new DynamicParameters(
                new RoleModel() {
                    GuildId = (long)Guild.Id,
                    RoleId = (long)role.Role.Id,
                    Description = role.Description == string.Empty ? null : role.Description,
                    RoleEmoji = role.Emoji.GetDiscordName(),
                    Mode = (int)DataMode.ADD
            })).Result;

            if (result.Status == StatusCodes.ERROR)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Remove(RoleComponent role) {
        try {
            var result = SaveData("Roles_Process", new DynamicParameters(
                new RoleModel() {
                    GuildId = (long)Guild.Id,
                    RoleId = (long)role.Role.Id,
                    Mode = (int)DataMode.DELETE
            })).Result;

            if (result.Status == StatusCodes.ERROR)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public bool HasRole(DiscordMember member, RoleComponent role) => member.Roles.Contains(role.Role);
    #endregion

    #region DB Methods
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
}

using DSharpPlus.Entities;
using System.Reflection;
using Dapper;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class ServerRoles : DataAccess
{
    #region Properties
    public DiscordGuild Guild { get; }
    public string CategoryName { get; private set; } = string.Empty;
    public string CategoryDescription { get; private set; } = string.Empty;
    public bool AllowMultipleRoles { get; private set; }
    public bool IsNotSetup => CategoryName == string.Empty && Roles.Count == 0;
    public List<RoleComponent> Roles { get; private set; } = [];
    public static DiscordEmoji DefaultEmoji => DiscordEmoji.FromName(DiscordHelper.Client, ":blue_circle:", false);

    #endregion
    
    #region Public Methods
    public ServerRoles(DiscordGuild guild) 
    {
        Guild = guild;
        Load();
    }

    public sealed override void Load()
    {
        Roles = [];
        var config = new ServerConfig(Guild);
        var data = GetData<RoleModel>(StoredProcedures.GET_ROLE_DATA, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;

        foreach (var role in data)
        {
            Roles.Add(new RoleComponent(Guild.GetRole((ulong)role.RoleId), role.Description ?? string.Empty, DiscordEmoji.FromName(DiscordHelper.Client, role.RoleEmoji ?? DefaultEmoji.Name)));
        }
        CategoryName = config.ServerRolesName ?? string.Empty;
        CategoryDescription = config.ServerRolesDescription ?? string.Empty;
        AllowMultipleRoles = config.AllowMultipleRoles;
    }

    public RoleComponent? this[DiscordRole role] => Roles.FirstOrDefault(x => x.Role == role);

    public async Task Add(RoleComponent role)
    {
        await SaveData(StoredProcedures.PROCESS_ROLE_DATA, new DynamicParameters(new RoleModel(Guild, role)));
        Load();
    }

    public async Task Remove(RoleComponent role)
    {
        await SaveData(StoredProcedures.REMOVE_ROLE_DATA, new DynamicParameters(new RoleModel(Guild, role)));
        Load();
    }

    public bool HasRole(DiscordMember member, RoleComponent role) => member.Roles.Contains(role.Role);
    #endregion
    
    public class RoleComponent(DiscordRole role, string desc, DiscordEmoji emoji)
    {
        public DiscordRole Role { get; set; } = role;
        public string Description { get; set; } = desc;
        public DiscordEmoji Emoji { get; set; } = emoji;
    }
}

using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class RoleProcessor {
    public static List<RoleModel> LoadRoles(ulong guildid) {
        string sql = @$"select RoleId, Description, Emoji from dbo.Roles where GuildId={guildid};";
        return SqlDataAccess.LoadData<RoleModel>(sql);
    }

    public static int SaveRole(ulong guildid, ulong roleid, string? desc, string? emoji) {
        string sql = @$"insert into dbo.Roles values ({guildid}, {roleid}, '{desc}', '{emoji}');";
        return SqlDataAccess.SaveData(sql, new RoleModel());
    }

    public static int DeleteRole(ulong guildid, ulong roleid) {
        string sql = @$"delete from dbo.Roles where GuildId={guildid} and RoleId={roleid};";
        return SqlDataAccess.SaveData(sql, new RoleModel());
    }
}

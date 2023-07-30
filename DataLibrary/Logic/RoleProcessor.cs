using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class RoleProcessor {
    public static List<RoleModel> LoadRoles(ulong guildid) {
        string sql = @$"select RoleId from dbo.Roles where GuildId={guildid};";
        return SqlDataAccess.LoadData<RoleModel>(sql);
    }

    public static int SaveRole(ulong guildid, ulong roleid) {
        string sql = @$"insert into dbo.Roles values ({guildid}, {roleid});";
        return SqlDataAccess.SaveData<RoleModel>(sql, new RoleModel());
    }

    public static int DeleteRole(ulong guildid, ulong roleid) {
        string sql = @$"delete from dbo.Roles where GuildId={guildid} and RoleId={roleid};";
        return SqlDataAccess.SaveData<RoleModel>(sql, new RoleModel());
    }
}

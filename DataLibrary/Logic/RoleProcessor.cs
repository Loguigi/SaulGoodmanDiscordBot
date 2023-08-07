using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class RoleProcessor {
    public static List<RoleModel> LoadRoles(ulong guildid) {
        string sql = @$"select RoleId, Description, RoleEmoji from dbo.Roles where GuildId={guildid};";
        return SqlDataAccess.LoadData<RoleModel>(sql);
    }

    public static int SaveRole(ulong guildid, ulong roleid, string? desc, string? emoji) {
        var role = new RoleModel() {
            GuildId = (long)guildid,
            RoleId = (long)roleid,
            Description = desc,
            RoleEmoji = emoji
        };
        string sql = @"insert into dbo.Roles values (@GuildId, @RoleId, @Description, @RoleEmoji);";
        return SqlDataAccess.SaveData(sql, role);
    }

    public static int DeleteRole(ulong guildid, ulong roleid) {
        var role = new RoleModel() {
            GuildId = (long)guildid,
            RoleId = (long)roleid
        };
        string sql = @"delete from dbo.Roles where GuildId=@GuildId and RoleId=@RoleId;";
        return SqlDataAccess.SaveData(sql, role);
    }
}

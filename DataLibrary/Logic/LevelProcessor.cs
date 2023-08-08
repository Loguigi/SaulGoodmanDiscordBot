using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class LevelProcessor {
    public static List<LevelModel> LoadUser(ulong guildid, ulong userid) {
        string sql = @$"select * from dbo.Levels where GuildId={guildid} and UserId={userid};";
        return SqlDataAccess.LoadData<LevelModel>(sql);
    }

    public static int SaveNewUser(ulong guildid, ulong userid, DateTime msglastsent) {
        var user = new LevelModel() {
            GuildId = (long)guildid,
            UserId = (long)userid,
            MsgLastSent = msglastsent
        };
        string sql = @"insert into dbo.Levels values (@GuildId, @UserId, @Level, @Experience, @MsgLastSent);";
        return SqlDataAccess.SaveData(sql, user);
    }

    public static int UpdateExp(ulong guildid, ulong userid, int level, int experience, DateTime msglastsent) {
        var user = new LevelModel() {
            GuildId = (long)guildid,
            UserId = (long)userid,
            Level = level,
            Experience = experience,
            MsgLastSent = msglastsent
        };
        string sql = @"update dbo.Levels set Level=@Level, Experience=@Experience, MsgLastSent=@MsgLastSent where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, user);
    }
}

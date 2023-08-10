using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class LevelProcessor {
    public static List<LevelModel> LoadUser(ulong guildid, ulong userid) {
        string sql = @$"select * from dbo.Levels where GuildId={guildid} and UserId={userid};";
        return SqlDataAccess.LoadData<LevelModel>(sql);
    }

    public static int SaveNewUser(ulong guildid, ulong userid, DateTimeOffset msglastsent) {
        var user = new LevelModel() {
            GuildId = (long)guildid,
            UserId = (long)userid,
            MsgLastSent = msglastsent.DateTime
        };
        string sql = @"insert into dbo.Levels values (@GuildId, @UserId, @Level, @Experience, @MsgLastSent);";
        return SqlDataAccess.SaveData(sql, user);
    }

    public static int UpdateExp(ulong guildid, ulong userid, int level, int experience, DateTimeOffset msglastsent) {
        var user = new LevelModel() {
            GuildId = (long)guildid,
            UserId = (long)userid,
            Level = level,
            Experience = experience,
            MsgLastSent = msglastsent.DateTime
        };
        string sql = @"update dbo.Levels set Level=@Level, Experience=@Experience, MsgLastSent=@MsgLastSent where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, user);
    }

    public static int GetRank(ulong guildid, ulong userid) {
        string sql = $"select * from dbo.Levels where GuildId={guildid} order by Level desc, Experience desc;";
        var rankList = SqlDataAccess.LoadData<LevelModel>(sql);
        return rankList.FindIndex(x => x.UserId == (long)userid) + 1;
    }
}

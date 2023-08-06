using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class BirthdayProcessor {
    public static int AddBirthday(ulong guildid, ulong userid, DateTime birthday) {
        var bday = new BirthdayModel() {
            GuildId = (long)guildid,
            UserId = (long)userid,
            Birthday = birthday
        };
        string sql = @"insert into dbo.Birthdays values (@GuildId, @UserId, @Birthday);";
        return SqlDataAccess.SaveData(sql, bday);
    }

    public static List<BirthdayModel> LoadBirthdays(ulong guildid) {
        string sql = @$"select UserId, Birthday from dbo.Birthdays where GuildId={guildid};";
        return SqlDataAccess.LoadData<BirthdayModel>(sql);
    }

    public static int UpdateBirthday(ulong guildid, ulong userid, DateTime birthday) {
        var bday = new BirthdayModel() {
            GuildId = (long)guildid,
            UserId = (long)userid,
            Birthday = birthday
        };
        string sql = @"update dbo.Birthdays set Birthday=@Birthday where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, bday);
    }
}
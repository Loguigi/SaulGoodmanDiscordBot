using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class BirthdayProcessor {
    public static int AddBirthday(ulong guildid, ulong userid, DateTime birthday) {
        string sql = @$"insert into dbo.Birthdays values ({guildid}, {userid}, '{birthday}');";
        return SqlDataAccess.SaveData<BirthdayModel>(sql, new BirthdayModel());
    }

    public static List<BirthdayModel> LoadBirthdays(ulong guildid) {
        string sql = @$"select UserId, Birthday from dbo.Birthdays where GuildId={guildid};";
        return SqlDataAccess.LoadData<BirthdayModel>(sql);
    }

    public static int UpdateBirthday(ulong guildid, ulong userid, DateTime birthday) {
        string sql = @$"update dbo.Birthdays set Birthday='{birthday}' where GuildId={guildid} and UserId={userid};";
        return SqlDataAccess.SaveData<BirthdayModel>(sql, new BirthdayModel());
    }
}
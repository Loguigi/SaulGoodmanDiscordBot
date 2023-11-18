using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class BirthdayProcessor {
    public static int AddBirthday(BirthdayModel birthday) {
        string sql = @"insert into dbo.Birthdays values (@GuildId, @UserId, @Birthday);";
        return SqlDataAccess.SaveData(sql, birthday);
    }

    public static List<BirthdayModel> LoadBirthdays(ulong guildid) {
        string sql = $"select * from dbo.Birthdays where GuildId={guildid};";
        return SqlDataAccess.LoadData<BirthdayModel>(sql);
    }

    public static int UpdateBirthday(BirthdayModel birthday) {
        string sql = @"update dbo.Birthdays set Birthday=@Birthday where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, birthday);
    }

    public static int RemoveBirthday(BirthdayModel birthday) {
        string sql = @"delete from dbo.Birthdays where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, birthday);
    }
}
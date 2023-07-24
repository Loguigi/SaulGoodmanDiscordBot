using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class BirthdayProcessor {
    public static int AddBirthday(ulong guildid, ulong userid, DateOnly birthday) {
        string sql = @$"insert into dbo.Birthdays values ({guildid}, {userid}, {birthday});";
        return SqlDataAccess.SaveData<BirthdayModel>(sql, new BirthdayModel());
    }
}
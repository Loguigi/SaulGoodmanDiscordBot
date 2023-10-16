using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class WheelPickerProcessor {
    public static int AddWheelOption(WheelPickerModel wheel) {
        string sql = @"insert into dbo.Wheels values (@GuildId, @WheelName, @WheelOption, @ImageUrl, @TempRemoved);";
        return SqlDataAccess.SaveData(sql, wheel);
    }

    public static List<WheelPickerModel> LoadAllWheels(ulong guildid) {
        string sql = @$"select * from dbo.Wheels where GuildId={guildid};";
        return SqlDataAccess.LoadData<WheelPickerModel>(sql);
    }

    public static int DeleteWheelOption(WheelPickerModel wheel) {
        string sql = @"delete from dbo.Wheels where GuildId=@GuildId and WheelName=@WheelName and WheelOption=@WheelOption;";
        return SqlDataAccess.SaveData(sql, wheel);
    }

    public static int DeleteWheel(WheelPickerModel wheel) {
        string sql = @"delete from dbo.Wheels where GuildId=@GuildId and WheelName=@WheelName;";
        return SqlDataAccess.SaveData(sql, wheel);
    }

    public static int TemporarilyRemoveOption(WheelPickerModel wheel) {
        string sql = @"update dbo.Wheels set TempRemoved=@TempRemoved where GuildId=@GuildId and WheelName=@WheelName and WheelOption=@WheelOption;";
        return SqlDataAccess.SaveData(sql, wheel);
    }

    public static int ReloadWheel(WheelPickerModel wheel) {
        string sql = @"update dbo.Wheels set TempRemoved=@TempRemoved where GuildId=@GuildId and WheelName=@WheelName;";
        return SqlDataAccess.SaveData(sql, wheel);
    }
}
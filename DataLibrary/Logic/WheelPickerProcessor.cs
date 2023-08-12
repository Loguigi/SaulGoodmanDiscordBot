using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class WheelPickerProcessor {
    public static int AddWheelOption(ulong guildid, string name, string option, string? imgurl) {
        var wheel = new WheelPickerModel() {
            GuildId = (long)guildid,
            WheelName = name,
            WheelOption = option,
            ImageUrl = imgurl
        };
        string sql = @"insert into dbo.Wheels values (@GuildId, @WheelName, @WheelOption, @ImageUrl);";
        return SqlDataAccess.SaveData(sql, wheel);
    }

    public static List<WheelPickerModel> LoadAllWheels(ulong guildid) {
        string sql = @$"select * from dbo.Wheels where GuildId={guildid};";
        return SqlDataAccess.LoadData<WheelPickerModel>(sql);
    }

    public static int DeleteWheelOption(ulong guildid, string name, string option) {
        var wheel = new WheelPickerModel() {
            GuildId = (long)guildid,
            WheelName = name,
            WheelOption = option
        };
        string sql = @$"delete from dbo.Wheels where GuildId=@GuildId and WheelName=@WheelName and WheelOption=@WheelOption;";
        return SqlDataAccess.SaveData(sql, wheel);
    }

    public static int DeleteWheel(ulong guildid, string name) {
        var wheel = new WheelPickerModel() {
            GuildId = (long)guildid,
            WheelName = name,
            WheelOption = ""
        };
        string sql = @$"delete from dbo.Wheels where GuildId=@GuildId and WheelName=@WheelName;";
        return SqlDataAccess.SaveData(sql, wheel);
    }
}
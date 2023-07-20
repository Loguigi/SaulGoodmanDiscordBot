using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class WheelPickerProcessor {
    public static int AddWheelOption(ulong guildid, string name, string option) {
        var data = new WheelPickerModel(guildid, name, option);
        string sql = @"insert into dbo.Wheels values @GuildId, @WheelName, @WheelOption";
        return SqlDataAccess.SaveData<WheelPickerModel>(sql, data);
    }

    public static List<WheelPickerModel> LoadWheel(ulong guildid, string name) {
        string sql = @$"select * from dbo.Wheels where GuildId={guildid} and WheelName='{name}';";
        return SqlDataAccess.LoadData<WheelPickerModel>(sql);
    }
}
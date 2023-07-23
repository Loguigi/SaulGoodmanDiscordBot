using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class WheelPickerProcessor {
    public static int AddWheelOption(ulong guildid, string name, string option, string? imgurl) {
        string sql = @$"insert into dbo.Wheels values ({guildid}, '{name}', '{option}', '{imgurl}');";
        return SqlDataAccess.SaveData<WheelPickerModel>(sql, new WheelPickerModel());
    }

    public static List<WheelPickerModel> LoadWheel(ulong guildid, string name) {
        string sql = @$"select WheelName, WheelOption, ImageUrl from dbo.Wheels where GuildId={guildid} and WheelName='{name}';";
        return SqlDataAccess.LoadData<WheelPickerModel>(sql);
    }

    public static List<WheelPickerModel> LoadAllWheels(ulong guildid) {
        string sql = @$"select WheelName, WheelOption from dbo.Wheels where GuildId={guildid};";
        return SqlDataAccess.LoadData<WheelPickerModel>(sql);
    }

    public static int DeleteWheelOption(ulong guildid, string name, string option) {
        string sql = @$"delete from dbo.Wheels where GuildId={guildid} and WheelName='{name}' and WheelOption='{option}';";
        return SqlDataAccess.SaveData<WheelPickerModel>(sql, new WheelPickerModel());
    }
}
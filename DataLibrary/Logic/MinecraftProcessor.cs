using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class MinecraftProcessor {
    public static List<MinecraftInfoModel> LoadMcInfo(ulong guildid) {
        string sql = @$"select * from dbo.MinecraftInfo where GuildId={guildid};";
        return SqlDataAccess.LoadData<MinecraftInfoModel>(sql);
    }

    public static int SaveMcInfo(MinecraftInfoModel info) {
        string sql = @"insert into dbo.MinecraftInfo values (@GuildId, @WorldName, @WorldDescription, @IPAddress, @MaxPlayers, @Whitelist);";
        return SqlDataAccess.SaveData(sql, info);
    }

    public static List<MinecraftCoordModel> LoadAllCoords(ulong guildid) {
        string sql = @$"select * from dbo.MinecraftCoords where GuildId={guildid};";
        return SqlDataAccess.LoadData<MinecraftCoordModel>(sql);
    }

    public static int SaveCoord(MinecraftCoordModel coord) {
        string sql = @"insert into dbo.MinecraftCoords values (@GuildId, @Dimension, @Name, @XCord, @YCord, @ZCord)";
        return SqlDataAccess.SaveData(sql, coord);
    }
}

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

    public static int UpdateMcInfo(MinecraftInfoModel info) {
        string sql = @"update dbo.MinecraftInfo set WorldName=@WorldName, WorldDescription=@WorldDescription, IPAddress=@IPAddress, MaxPlayers=@MaxPlayers, @WhiteList=Whitelist where GuildId=@GuildId;";
        return SqlDataAccess.SaveData(sql, info);
    }

    public static List<MinecraftWaypointModel> LoadAllWaypoints(ulong guildid) {
        string sql = @$"select * from dbo.MinecraftWaypoint where GuildId={guildid};";
        return SqlDataAccess.LoadData<MinecraftWaypointModel>(sql);
    }

    public static int SaveWaypoint(MinecraftWaypointModel coord) {
        string sql = @"insert into dbo.MinecraftWaypoint values (@GuildId, @Dimension, @Name, @XCord, @YCord, @ZCord)";
        return SqlDataAccess.SaveData(sql, coord);
    }
}

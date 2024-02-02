using System.Runtime.CompilerServices;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library.Minecraft;

public class McConfig : DbBase<MinecraftInfoModel, McConfig> {

    public McConfig(DiscordGuild guild)
    {
        Guild = guild;
    }

    #region DB Methods
    protected override ResultArgs<List<MinecraftInfoModel>> GetData(string sp)
    {
        throw new NotImplementedException();
    }

    protected override ResultArgs<int> SaveData(string sp, MinecraftInfoModel data)
    {
        throw new NotImplementedException();
    }

    protected override List<McConfig> MapData(List<MinecraftInfoModel> data)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Properties
    private DiscordGuild Guild { get; set; }
    public string WorldName { get; set; } = string.Empty;
    public string? WorldDescription { get; set; } = null;
    public string? IPAddress { get; set; } = null;
    public int? MaxPlayers { get; set; } = null;
    public bool Whitelist { get; set; } = false;
    #endregion
} 
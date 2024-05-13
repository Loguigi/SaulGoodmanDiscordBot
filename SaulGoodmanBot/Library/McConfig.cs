using System.Runtime.CompilerServices;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class McConfig {
    #region Properties
    public string WorldName { get; set; } = string.Empty;
    public string? WorldDescription { get; set; } = null;
    public string? IPAddress { get; set; } = null;
    public int? MaxPlayers { get; set; } = null;
    public bool Whitelist { get; set; } = false;
    #endregion

    public McConfig() {}
    public McConfig(MinecraftInfoModel model) {
        WorldName = model.WorldName ?? string.Empty;
        WorldDescription = model.WorldDescription;
        IPAddress = model.IPAddress;
        MaxPlayers = model.MaxPlayers;
        Whitelist = model.Whitelist == 1;
    }
} 
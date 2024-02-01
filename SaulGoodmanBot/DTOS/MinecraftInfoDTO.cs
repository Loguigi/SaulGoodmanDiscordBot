using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.DTO;

public class MinecraftInfoDTO : DbCommonParams {
    public long GuildId { get; set; }
    public string WorldName { get; set; } = string.Empty;
    public string? WorldDescription { get; set; } = null;
    public string? IPAddress { get; set; } = null;
    public int? MaxPlayers { get; set; } = null;
    public int Whitelist { get; set; } = 0;
}
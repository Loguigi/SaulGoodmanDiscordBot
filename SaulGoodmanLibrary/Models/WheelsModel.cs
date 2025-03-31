using DSharpPlus.Entities;

namespace SaulGoodmanLibrary.Models;

public class WheelsModel 
{
    public long GuildId { get; set; }
    public string WheelName { get; set; } = string.Empty;
    public string WheelOption { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = null;
    public int TempRemoved { get; set; } = 0;
    
    public WheelsModel() { }

    public WheelsModel(WheelPickers.Wheel wheel, DiscordGuild guild, string option = "", bool tempRemoved = false)
    {
        GuildId = (long)guild.Id;
        WheelName = wheel.Name;
        WheelOption = option;
        ImageUrl = wheel.Image == string.Empty ? null : wheel.Image;
        TempRemoved = tempRemoved ? 1 : 0;
    }
}
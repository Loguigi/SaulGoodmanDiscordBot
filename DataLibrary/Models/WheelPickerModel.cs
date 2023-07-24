namespace DataLibrary.Models;

public class WheelPickerModel {
    public ulong GuildId { get; set; }
    public string WheelName { get; set; } = "";
    public string WheelOption { get; set; } = "";
    public string? ImageUrl { get; set; } = null;
}
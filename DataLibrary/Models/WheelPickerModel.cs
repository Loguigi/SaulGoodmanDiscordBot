using DSharpPlus;
using DSharpPlus.Entities;

namespace DataLibrary.Models;

public class WheelPickerModel {
    public ulong GuildId { get; set; } = 0;
    public string WheelName { get; set; } = "";
    public string WheelOption { get; set; } = "";

    public WheelPickerModel(ulong guildid, string name, string option) {
        GuildId = guildid;
        WheelName = name;
        WheelOption = option;
    }
}
using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;

namespace SaulGoodmanBot.Library.WheelPicker;

public class WheelPickers {
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        var data = WheelPickerProcessor.LoadAllWheels(Guild.Id);

        foreach (var row in data) {
            if (Wheels.ContainsKey(row.WheelName)) {
                if (row.TempRemoved == 1) {
                    Wheels[row.WheelName].RemovedOptions.Add(row.WheelOption);
                } else {
                    Wheels[row.WheelName].Options.Add(row.WheelOption);
                }
                Wheels[row.WheelName].Image = row.ImageUrl;
            } else {
                if (row.TempRemoved == 1) {
                    Wheels[row.WheelName] = new Wheel(Guild, row.WheelName, new List<string>(), new List<string>(){row.WheelOption}, row.ImageUrl);
                } else {
                    Wheels[row.WheelName] = new Wheel(Guild, row.WheelName, new List<string>(){row.WheelOption}, new List<string>(), row.ImageUrl);
                }
            }
        }
    }

    public void AddWheel(string name, string first_option, string? imgurl) {
        WheelPickerProcessor.AddWheelOption(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = name,
            WheelOption = first_option,
            ImageUrl = imgurl
        });
    }

    public bool Contains(string name) {
        return Wheels.ContainsKey(name);
    }

    public void DeleteWheel(Wheel wheel) {
        WheelPickerProcessor.DeleteWheel(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = wheel.Name,
        });
    }

    public bool IsFull() {
        return Wheels.Count == WHEEL_LIMIT;
    }

    private DiscordGuild Guild { get; set; }
    public Dictionary<string, Wheel> Wheels = new();
    private const int WHEEL_LIMIT = 25;
}

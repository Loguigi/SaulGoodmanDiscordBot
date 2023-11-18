using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;

namespace SaulGoodmanBot.Library.WheelPicker;

public class Wheel {
    public Wheel(DiscordGuild guild, string name, List<string> options, List<string> removedOptions, string? imgurl=null) {
        Guild = guild;
        Name = name;
        Options = options;
        RemovedOptions = removedOptions;
        Image = imgurl;
    }

    public string Spin() {
        var random = new Random();
        var i = random.Next(Options.Count);
        return Options[i];
    }

    public void AddOption(string option) {
        WheelPickerProcessor.AddWheelOption(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = Name,
            WheelOption = option,
            ImageUrl = Image
        });
        Options.Add(option);
    }

    public void DeleteOption(string option) {
        WheelPickerProcessor.DeleteWheelOption(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = Name,
            WheelOption = option
        });
    }

    public void TemporarilyRemoveOption(string option) {
        WheelPickerProcessor.TemporarilyRemoveOption(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = Name,
            WheelOption = option,
            TempRemoved = 1
        });
        Options.Remove(option);
        RemovedOptions.Add(option);
    }

    public void Reload() {
        WheelPickerProcessor.ReloadWheel(new WheelPickerModel() {
            GuildId = (long)Guild.Id,
            WheelName = Name,
            TempRemoved = 0
        });
    }

    public List<string> GetAllOptions() {
        return Options.Concat(RemovedOptions).ToList();
    }

    private DiscordGuild Guild { get; set; }
    public string Name { get; private set; }
    public List<string> Options { get; private set; }
    public List<string> RemovedOptions { get; private set; }
    public string? Image { get; set; } = null;
}
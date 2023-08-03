using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class WheelPickers {
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        var data = WheelPickerProcessor.LoadAllWheels(Guild.Id);

        foreach (var row in data) {
            if (Wheels.ContainsKey(row.WheelName)) {
                Wheels[row.WheelName].AddOption(row.WheelOption);
                if (row.ImageUrl != null) Wheels[row.WheelName].Image = row.ImageUrl;
            } else {
                Wheels[row.WheelName] = new Wheel(row.WheelName, new List<string>(){row.WheelOption}, row.ImageUrl);
            }
        }
    }

    public bool Contains(string name) {
        return Wheels.ContainsKey(name);
    }

    public void Add(Wheel wheel) {
        foreach (var option in wheel.Options) {
            WheelPickerProcessor.AddWheelOption(Guild.Id, wheel.Name, option, wheel.Image);
        }
    }

    public void Delete(Wheel wheel, string option="") {
        if (option != "") {
            // delete wheel option
            WheelPickerProcessor.DeleteWheelOption(Guild.Id, wheel.Name, option);
        } else {
            // delete entire wheel
            WheelPickerProcessor.DeleteWheel(Guild.Id, wheel.Name);
        }
    }

    public bool IsFull() {
        return Wheels.Count == WHEEL_LIMIT;
    }

    private DiscordGuild Guild { get; set; }
    public Dictionary<string, Wheel> Wheels { get; set; } = new Dictionary<string, Wheel>();
    private const int WHEEL_LIMIT = 20;
}

public class Wheel {
    public Wheel(string name, List<string> options, string? imgurl=null) {
        Name = name;
        Options = options;
        Image = imgurl;
    }

    public void AddOption(string option) {
        Options.Add(option);
    }

    public string Spin() {
        var random = new Random();
        var i = random.Next(Options.Count);
        return Options[i];
    }

    public string Name { get; private set; }
    public List<string> Options { get; private set; }
    public string? Image { get; set; } = null;
}

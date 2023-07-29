using System.Text.Json;
using System.Text.Json.Serialization;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class WheelPickers {
    public WheelPickers(ulong guildid) {
        GuildId = guildid;
        var data = WheelPickerProcessor.LoadAllWheels(GuildId);

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
            WheelPickerProcessor.AddWheelOption(GuildId, wheel.Name, option, wheel.Image);
        }
    }

    public void Delete(Wheel wheel, string option="") {
        if (option != "") {
            // delete wheel option
            WheelPickerProcessor.DeleteWheelOption(GuildId, wheel.Name, option);
        } else {
            // delete entire wheel
            WheelPickerProcessor.DeleteWheel(GuildId, wheel.Name);
        }
    }

    public Dictionary<string, int> List() {
        var wheels = new Dictionary<string, int>();
        foreach (var wheel in Wheels) {
            wheels[wheel.Key] = wheel.Value.Options.Count;
        }
        return wheels;
    }

    private ulong GuildId { get; set; }
    public Dictionary<string, Wheel> Wheels { get; set; } = new Dictionary<string, Wheel>();
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

    public DiscordMessageBuilder Spin() {
        var random = new Random();
        var i = random.Next(Options.Count);

        var response = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"Spinning {Name}...")
                .WithDescription($"## {Options[i]}")
                .WithThumbnail(Image)
                .WithColor(DiscordColor.Gold));

        return response;
            
    }

    public string Name { get; private set; }
    public List<string> Options { get; private set; }
    public string? Image { get; set; } = null;
}

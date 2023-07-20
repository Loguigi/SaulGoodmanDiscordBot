using System.Text.Json;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class WheelPicker {
    public DiscordGuild Server { get; set; }
    public string WheelName { get; set; }
    public List<String> WheelOptions { get; set; }

    public WheelPicker(DiscordGuild server, string name) {
        Server = server;
        WheelName = name;
        WheelOptions = new List<string>();

        var data = WheelPickerProcessor.LoadWheel(Server.Id, WheelName);
        foreach (var row in data) {
            WheelOptions.Add(row.WheelOption);
        }
    }

    public WheelPicker(DiscordGuild server, string name, List<String> options) {
        Server = server;
        WheelName = name;
        WheelOptions = options;
    }

    public void SaveWheel() {
        foreach (var option in WheelOptions) {
            WheelPickerProcessor.AddWheelOption(Server.Id, WheelName, option);
        }
    }

    public string SpinWheel() {
        var random = new Random();
        var i = random.Next(WheelOptions.Count);
        return WheelOptions[i];
    }
}
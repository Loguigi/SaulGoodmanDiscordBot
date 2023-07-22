using System.Text.Json;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class WheelPicker {
    private DiscordGuild Server { get; set; }
    public string WheelName { get; private set; } = "";
    private List<String> WheelOptions { get; set; } = new List<string>();
    private string? WheelImage { get; set; } = null;
    private bool IsNewWheel { get; set; } = false;
    private const int WheelLimit = 20;

    public WheelPicker(DiscordGuild server) {
        Server = server;
    }

    public WheelPicker(DiscordGuild server, string name) {
        Server = server;
        WheelName = name;
        IsNewWheel = false;

        var data = WheelPickerProcessor.LoadWheel(Server.Id, WheelName);
        foreach (var row in data) {
            WheelOptions.Add(row.WheelOption);
            if (row.ImageUrl != null) WheelImage = row.ImageUrl;
        }
    }

    public WheelPicker(DiscordGuild server, string name, string? pic, List<String> options) {
        Server = server;
        WheelName = name;
        WheelOptions = options;
        WheelImage = pic;
        IsNewWheel = true;
    }

    public void SaveWheel() {
        foreach (var option in WheelOptions) {
            WheelPickerProcessor.AddWheelOption(Server.Id, WheelName, option, WheelImage);
        }
    }

    public string Spin() {
        var random = new Random();
        var i = random.Next(WheelOptions.Count);
        return WheelOptions[i];
    }

    public List<string> GetOptions() {
        return WheelOptions;
    }

    public Dictionary<string, int>? GetAllWheels() {
        var wheels = new Dictionary<string, int>();
        var data = WheelPickerProcessor.LoadAllWheels(Server.Id);

        if (data.Count == 0) return null;

        foreach (var row in data) {
            if (wheels.ContainsKey(row.WheelName))
                wheels[row.WheelName]++;
            else
                wheels.Add(row.WheelName, 1);
        }

        return wheels;
    }

    public string? GetImgUrl() {
        return WheelImage;
    }

    public bool Exists() {
        if (IsNewWheel) {
            var data = WheelPickerProcessor.LoadWheel(Server.Id, WheelName);
            return (data.Count > 0) ? true : false;
        } else {
            return (WheelOptions.Count > 0) ? true : false;
        }
        
    }

    // public bool AtLimit() {
    //     return (WheelOptions.Count)
    // }
}
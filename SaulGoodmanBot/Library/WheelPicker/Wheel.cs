using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Library.WheelPicker;

internal class Wheel {
    public Wheel(string name, string imgurl) {
        Name = name;
        Image = imgurl;
    }

    public string Spin() {
        var i = RandomHelper.RNG.Next(AvailableOptions.Count);
        return AvailableOptions[i];
    }



    public string Name { get; private set; }
    public List<string> AvailableOptions { get; private set; } = new();
    public List<string> RemovedOptions { get; private set; } = new();
    public List<string> Options {
        get => AvailableOptions.Concat(RemovedOptions).ToList();
    }
    public string Image { get; set; } = string.Empty;
}
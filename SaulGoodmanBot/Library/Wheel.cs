using SaulGoodmanBot.Helpers;

namespace SaulGoodmanBot.Library;

public class Wheel {
    #region Properties
    public string Name { get; private set; }
    public List<string> AvailableOptions { get; private set; } = new();
    public List<string> RemovedOptions { get; private set; } = new();
    public List<string> Options {
        get => AvailableOptions.Concat(RemovedOptions).ToList();
    }
    public string Image { get; set; } = string.Empty;
    #endregion
    
    public Wheel(string name, string imgurl) {
        Name = name;
        Image = imgurl;
    }

    public string Spin() {
        var i = RandomHelper.RNG.Next(AvailableOptions.Count);
        return AvailableOptions[i];
    }
}
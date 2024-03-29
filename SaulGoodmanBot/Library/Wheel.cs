using SaulGoodmanBot.Helpers;

namespace SaulGoodmanBot.Library;

public class Wheel(string name, string imgurl)
{
    #region Properties
    public string Name { get; private set; } = name;
    public List<string> AvailableOptions { get; private set; } = [];
    public List<string> RemovedOptions { get; private set; } = [];
    public List<string> Options => [.. AvailableOptions, .. RemovedOptions];
    public string Image { get; set; } = imgurl;

    #endregion

    public string Spin() {
        var i = RandomHelper.RNG.Next(AvailableOptions.Count);
        return AvailableOptions[i];
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SaulGoodmanBot.Library;

public class WheelPicker {
    public string WheelName { get; set; } = "";
    public List<String> WheelOptions { get; set; } = new List<string>();
}
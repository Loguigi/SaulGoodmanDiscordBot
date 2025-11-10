namespace GarryLibrary.Helpers;

public record SpinData(string WheelName, int SpinCount, string LastOptionSpun, string? PreviousOptionSpun, bool ShouldRemoveLastOption)
{
    public string ToButtonId() => $@"{IDHelper.WheelPicker.Spin}\{WheelName}\{SpinCount}\{LastOptionSpun}\{PreviousOptionSpun ?? "null"}\{ShouldRemoveLastOption}";

    public SpinData IncrementSpin(string newResult) => new SpinData(
        WheelName, 
        SpinCount + 1, 
        newResult, 
        LastOptionSpun,  // Current becomes previous
        false
    );
    
    public static SpinData FromButtonId(string buttonId)
    {
        var parts = buttonId.Split('\\');
        var wheelName = parts[1];
        var spinCount = int.Parse(parts[2]);
        var lastOptionSpun = parts[3];
        var previousOptionSpun = parts[4] == "null" ? null : parts[4];
        var shouldRemoveLastOption = bool.Parse(parts[5]);
        
        return new SpinData(wheelName, spinCount, lastOptionSpun, previousOptionSpun, shouldRemoveLastOption);
    }
}
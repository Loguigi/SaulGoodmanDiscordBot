namespace GarryLibrary.Helpers;

public record SpinData(int WheelId, int SpinCount, string LastOptionSpun, string? PreviousOptionSpun, bool ShouldRemoveLastOption)
{
    public string ToButtonId() => $@"{IDHelper.WheelPicker.Spin}\{WheelId}\{SpinCount}\{LastOptionSpun}\{PreviousOptionSpun ?? "null"}\{ShouldRemoveLastOption}";

    public SpinData IncrementSpin(string newResult) => new(
        WheelId, 
        SpinCount + 1, 
        newResult, 
        LastOptionSpun,  // Current becomes previous
        ShouldRemoveLastOption
    );
    
    public static SpinData FromButtonId(string buttonId)
    {
        var parts = buttonId.Split('\\');
        var wheelId = int.Parse(parts[1]);
        var spinCount = int.Parse(parts[2]);
        var lastOptionSpun = parts[3];
        var previousOptionSpun = parts[4] == "null" ? null : parts[4];
        var shouldRemoveLastOption = bool.Parse(parts[5]);
        
        return new SpinData(wheelId, spinCount, lastOptionSpun, previousOptionSpun, shouldRemoveLastOption);
    }
}
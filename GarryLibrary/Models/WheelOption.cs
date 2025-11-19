namespace GarryLibrary.Models;

public class WheelOption
{
    public int WheelId { get; set; }
    public string Option { get; set; }
    public bool TempRemoved { get; set; }

    public WheelPicker WheelPicker { get; set; } = null!;
}
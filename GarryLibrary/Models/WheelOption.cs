using System.ComponentModel.DataAnnotations.Schema;
using GarryLibrary.Helpers;

namespace GarryLibrary.Models;

public class WheelOption : IPageable
{
    public int WheelId { get; set; }
    public string Option { get; set; }
    public bool TempRemoved { get; set; }
    public WheelPicker WheelPicker { get; set; } = null!;

    [NotMapped] public bool Indexed => true;
    public string GetPageItemDisplay(string context) => context switch
    {
        IDHelper.WheelPicker.List => TempRemoved ? $"~~{Option}~~" : Option,
        _ => string.Empty
    };
}
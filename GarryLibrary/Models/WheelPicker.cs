using System.ComponentModel.DataAnnotations.Schema;

namespace GarryLibrary.Models;

public class WheelPicker
{
    public int Id { get; set; }
    public long GuildId { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public List<WheelOption> WheelOptions { get; set; }
    
    [NotMapped] public List<WheelOption> AvailableOptions => WheelOptions.Where(x => !x.TempRemoved).ToList();
    [NotMapped] public List<WheelOption> TempRemovedOptions => WheelOptions.Where(x => x.TempRemoved).ToList();
    
    public WheelOption? Spin(Random random)
    {
        if (AvailableOptions.Count == 0)
            return null;

        var index = random.Next(AvailableOptions.Count);
        return AvailableOptions[index];
    }

    public WheelOption? SpinAndRemove(Random random)
    {
        var result = Spin(random);
        if (result != null)
        {
            result.TempRemoved = true;
        }
        return result;
    }
}
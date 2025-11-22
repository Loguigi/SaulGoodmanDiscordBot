namespace GarryLibrary.Helpers;

public record FlipData(FlipResult LastFlip, int HeadCount, int TailsCount)
{
    public string ToButtonId() => $@"{IDHelper.Misc.FLIP}\{LastFlip}\{HeadCount}\{TailsCount}";

    public FlipData Flip(FlipResult flip) => flip switch
    {
        FlipResult.Heads => new FlipData(FlipResult.Tails, HeadCount + 1, TailsCount),
        FlipResult.Tails => new FlipData(FlipResult.Heads, HeadCount, TailsCount + 1),
        _ => throw new ArgumentException("Invalid flip result")
    };
    
    public static FlipData FirstFlip(FlipResult flip) => flip switch
    {
        FlipResult.Heads => new FlipData(FlipResult.Heads, 1, 0),
        FlipResult.Tails => new FlipData(FlipResult.Tails, 0, 1),
        _ => throw new ArgumentException("Invalid flip result")
    };
    
    public static FlipData FromButtonId(string buttonId)
    {
        var parts = buttonId.Split('\\');
        var lastFlip = Enum.Parse<FlipResult>(parts[1]);
        var headsCount = int.Parse(parts[2]);
        var tailsCount = int.Parse(parts[3]);
        return new FlipData(lastFlip, headsCount, tailsCount);
    }
}

public enum FlipResult
{
    Heads,
    Tails
}
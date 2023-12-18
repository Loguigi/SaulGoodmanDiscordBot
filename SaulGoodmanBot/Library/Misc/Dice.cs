using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Library.Misc;

public class Dice {
    public Dice(DiceTypes type) {
        Type = type;
    }

    public int RollOnce() {
        return DiceInfo[Type][RandomHelper.RNG.Next(DiceInfo[Type].Count)];
    }

    public List<int> RollMultiple(int dice_count) {
        var rolls = new List<int>(dice_count);
        for (var i = 1; i <= dice_count; ++i) {
            rolls.Add(DiceInfo[Type][RandomHelper.RNG.Next(DiceInfo[Type].Count)]);
        }
        return rolls;
    }

    public int GetRollTotal(List<int> rolls) {
        int total = 0;
        rolls.ForEach(x => total += x);
        return total;
    }

    private static Dictionary<DiceTypes, List<int>> DiceInfo = new() {
        {DiceTypes.D4, Enumerable.Range(1, 4).ToList()},
        {DiceTypes.D6, Enumerable.Range(1, 6).ToList()},
        {DiceTypes.D8, Enumerable.Range(1, 8).ToList()},
        {DiceTypes.D10, Enumerable.Range(1, 10).ToList()},
        {DiceTypes.D100, new(){00, 10, 20, 30, 40, 50, 60, 70, 80, 90}},
        {DiceTypes.D12, Enumerable.Range(1, 12).ToList()},
        {DiceTypes.D20, Enumerable.Range(1, 20).ToList()}
    };

    public DiceTypes Type { get; set; }
}

public enum DiceTypes {
    [ChoiceName("d4")]
    D4,
    [ChoiceName("d6")]
    D6,
    [ChoiceName("d8")]
    D8,
    [ChoiceName("d10")]
    D10,
    [ChoiceName("d100")]
    D100,
    [ChoiceName("d12")]
    D12,
    [ChoiceName("d20")]
    D20
}
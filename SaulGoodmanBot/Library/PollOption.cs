using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class PollOption(DiscordEmoji emoji, string name, int votes)
{
    public DiscordEmoji Emoji { get; private set; } = emoji;
    public string Name { get; private set; } = name;
    public int Votes { get; set; } = votes;
}
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class PollOption {
    public DiscordEmoji Emoji { get; private set; }
    public string Name { get; private set; }
    public int Votes { get; set; }
    
    public PollOption(DiscordEmoji emoji, string name, int votes) {
        Emoji = emoji;
        Name = name;
        Votes = votes;
    }
}
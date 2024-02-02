using DSharpPlus;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Controllers;

public class Poll {
    public Poll(DiscordClient client, string question, TimeSpan time_limit, int num_options) {
        Client = client;
        Title = question;
        TimeLimit = time_limit;
        Options = new(num_options);
        Emojis = new();

        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":one:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":two:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":three:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":four:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":five:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":six:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":seven:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":eight:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":nine:"));
        Emojis.Enqueue(DiscordEmoji.FromName(Client, ":keycap_ten:"));
    }

    public void AddOption(string option) {
        Options.Add(new PollOption(Emojis.Dequeue(), option, 0));
    }

    public int GetTotalVotes() {
        int total = 0;
        Options.ForEach(delegate(PollOption option) {
            total += option.Votes;
        });
        return total;
    }

    public List<DiscordEmoji> GetEmojis() {
        var emojis = new List<DiscordEmoji>();
        foreach (var o in Options) {
            emojis.Add(o.Emoji);
        }
        return emojis;
    }

    public string FormatPercent(PollOption option) {
        return string.Format("{0:P1}", option.Votes / GetTotalVotes());
    }

    public DiscordClient Client { get; set; }
    public string Title { get; set; }
    public TimeSpan TimeLimit { get; set; }
    public List<PollOption> Options { get; set; }
    public Queue<DiscordEmoji> Emojis { get; }
}
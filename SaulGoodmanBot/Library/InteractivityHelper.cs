using System.Data;
using DSharpPlus;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class InteractivityHelper<T> {
    public InteractivityHelper(DiscordClient client, List<T> data, string customid, int page, string emptyMessage="") {
        Client = client;
        Data = data;
        CustomId = customid;
        Page = page;
        EmptyMessage = emptyMessage;
        PageLimit = Data.Count == 0 ? 1 : (int)Math.Ceiling((double)Data.Count / MAX_ENTIRES_PER_PAGE);
    }

    public List<T> GetPage() {
        if (Data.Count == 0)
            return new List<T>();

        var after_indexing = new List<T>();
        int lower_limit = (Page - 1) * MAX_ENTIRES_PER_PAGE;
        int upper_limit = Page * MAX_ENTIRES_PER_PAGE - 1;

        if (Page == PageLimit && Data.Count - 1 < upper_limit) {
            upper_limit = Data.Count - 1;
        }

        for (int i = lower_limit; i <= upper_limit; ++i) {
            after_indexing.Add(Data[i]);
        }

        return after_indexing;
    }

    public DiscordMessageBuilder AddPageButtons() {
        return new DiscordMessageBuilder().AddComponents(new List<DiscordButtonComponent>() {
            new(ButtonStyle.Primary, $"{CustomId}\\1", "", Page == 1, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:", false))),
            new(ButtonStyle.Primary, $"{CustomId}\\{Page - 1}", "", Page - 1 < 1, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":rewind:", false))),
            new(ButtonStyle.Primary, $"{CustomId}\\{Page + 1}", "", Page + 1 > PageLimit, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":fast_forward:", false))),
            new(ButtonStyle.Primary, $"{CustomId}\\{(PageLimit == 1 ? "last" : PageLimit)}", "", Page == PageLimit, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next:", false)))
        });
    }

    // intended for Discord embed description. empty string return signals data exists and ready to be added to description
    public string IsEmpty() {
        return Data.Count == 0 ? EmptyMessage : "";
    }

    private DiscordClient Client { get; set; }
    public List<T> Data { get; private set; }
    public string CustomId { get; set; }
    public int Page { get; set; }
    public int PageLimit { get; set; }
    public string EmptyMessage { get; set; }
    private const int MAX_ENTIRES_PER_PAGE = 10;
}
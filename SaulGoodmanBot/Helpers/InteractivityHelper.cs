using System.Collections;
using System.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace SaulGoodmanBot.Helpers;

public class InteractivityHelper<T> : IEnumerable<T> {
    public InteractivityHelper(DiscordClient client, List<T> data, string customid, string page, int countPerPage, string emptyMessage="") {
        Client = client;
        Data = data;
        PerPageCount = countPerPage;
        CustomId = customid;
        PageLimit = Data.Count == 0 ? 1 : (int)Math.Ceiling((double)Data.Count / PerPageCount);
        PageNum = ParsePage(page);
        EmptyMessage = emptyMessage;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() => GetPage().GetEnumerator();

    public List<T> GetPage() {
        if (Data.Count == 0)
            return new List<T>();

        var after_indexing = new List<T>();
        int lower_limit = (PageNum - 1) * PerPageCount;
        int upper_limit = PageNum * PerPageCount - 1;

        if (PageNum == PageLimit && Data.Count - 1 < upper_limit) {
            upper_limit = Data.Count - 1;
        }

        for (int i = lower_limit; i <= upper_limit; ++i) {
            after_indexing.Add(Data[i]);
        }

        return after_indexing;
    }

    public T this[int index] {
        get => GetPage()[index];
    }

    public DiscordMessageBuilder AddPageButtons() {
        return new DiscordMessageBuilder().AddComponents(new List<DiscordButtonComponent>() {
            new(ButtonStyle.Primary, $"{CustomId}\\{FIRST_PAGE}", "", PageNum == 1, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_previous:", false))),
            new(ButtonStyle.Primary, $"{CustomId}\\{PageNum - 1}", "", PageNum - 1 < 1, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":rewind:", false))),
            new(ButtonStyle.Primary, $"{CustomId}\\{PageNum + 1}", "", PageNum + 1 > PageLimit, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":fast_forward:", false))),
            new(ButtonStyle.Primary, $"{CustomId}\\{LAST_PAGE}", "", PageNum == PageLimit, new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":track_next:", false)))
        });
    }

    // intended for Discord embed description. empty string return signals data exists and ready to be added to description
    public string IsEmpty() {
        return Data.Count == 0 ? EmptyMessage : "";
    }

    public int ParsePage(string page) => page switch {
        FIRST_PAGE => 1,
        LAST_PAGE => PageLimit,
        _ => int.Parse(page)
    };

    private DiscordClient Client { get; set; }
    public List<T> Data { get; private set; }
    public string CustomId { get; set; }
    public int PageNum { get; set; }
    public int PerPageCount { get; set; }
    public int PageLimit { get; set; }
    public string PageStatus {
        get => $"Page {PageNum} of {PageLimit}";
    }
    private string EmptyMessage { get; set; }
    private const string FIRST_PAGE = "FIRSTPAGE";
    private const string LAST_PAGE = "LASTPAGE";
}
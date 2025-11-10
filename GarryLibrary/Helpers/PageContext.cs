using System.Text;
using DSharpPlus.Entities;
using GarryLibrary.Models;

namespace GarryLibrary.Helpers;

public class PageContext<T>(List<T> data, int itemsPerPage, string currentPage, string customId)
{
    private const string FIRST_PAGE = "FIRSTPAGE";
    private const string LAST_PAGE = "LASTPAGE";

    private int CurrentPage => currentPage switch
    {
        FIRST_PAGE => 1,
        LAST_PAGE => TotalPages,
        _ => int.Parse(currentPage)
    };
    private int TotalPages => data.Count == 0 ? 1 : (int)Math.Ceiling(data.Count / (double)itemsPerPage);
    
    public string PageStatus => $"Page {CurrentPage} of {TotalPages}";

    public string GetPageText()
    {
        StringBuilder text = new();
        
        if (data.Count == 0) return text.ToString();
        
        var pageItems = data
            .Skip((CurrentPage - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToList();

        foreach (var item in pageItems)
        {
            if (item is IPageable pageItem)
            {
                text.AppendLine(pageItem.GetPageItemDisplay(customId));
            }
        }
            
        
        return text.ToString();
    }

    public DiscordComponent[] GetPageButtons() =>
    [
        new DiscordButtonComponent(DiscordButtonStyle.Primary, $"{customId}\\{FIRST_PAGE}", "", CurrentPage == 1, new DiscordComponentEmoji("⏮️")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, $"{customId}\\{CurrentPage - 1}", "", CurrentPage - 1 < 1, new DiscordComponentEmoji("⏪")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, $"{customId}\\{CurrentPage + 1}", "", CurrentPage + 1 > TotalPages, new DiscordComponentEmoji("⏩")),
        new DiscordButtonComponent(DiscordButtonStyle.Primary, $"{customId}\\{LAST_PAGE}", "", CurrentPage == TotalPages, new DiscordComponentEmoji("⏭️"))
    ];

    private int ParsePage(string page) => page switch
    {
        FIRST_PAGE => 1,
        LAST_PAGE => TotalPages,
        _ => int.Parse(page)
    };
}
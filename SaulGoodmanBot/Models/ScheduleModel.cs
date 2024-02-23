using static SaulGoodmanBot.Data.Constants;

namespace SaulGoodmanBot.Models;

public class ScheduleModel {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public DateTime LastUpdated { get; set; } = DATE_ERROR;
    public int RecurringSchedule { get; set; } = 0;
    public string? Sunday { get; set; } = null;
    public string? Monday { get; set; } = null;
    public string? Tuesday { get; set; } = null;
    public string? Wednesday { get; set; } = null;
    public string? Thursday { get; set; } = null;
    public string? Friday { get; set; } = null;
    public string? Saturday { get; set; } = null;
    public string? PictureUrl { get; set; } = null;
    public int Mode { get; set; } = 0;

    public List<string?> ToList() {
        return new List<string?>() {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
        };
    }
}
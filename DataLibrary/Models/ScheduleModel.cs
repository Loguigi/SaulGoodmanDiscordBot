namespace DataLibrary.Models;

public class ScheduleModel {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public DateTime LastUpdated { get; set; }
    public int RecurringSchedule { get; set; } = 0;
    public string? Monday { get; set; }
    public string? Tuesday { get; set; }
    public string? Wednesday { get; set; }
    public string? Thursday { get; set; }
    public string? Friday { get; set; }
    public string? Saturday { get; set; }
    public string? Sunday { get; set; }
    public string? PictureUrl { get; set; }
}
namespace SaulGoodmanBot.Models;

public class TicketActionModel {
    public int TicketId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; }
}
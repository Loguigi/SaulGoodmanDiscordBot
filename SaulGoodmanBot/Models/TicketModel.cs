namespace SaulGoodmanBot.Models;

public class TicketModel {
    public int Id { get; set; }
    public long SubmittedBy { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime DateSubmitted { get; set; }
    public int TicketType { get; set; }
    public int TicketStatus { get; set; }
}
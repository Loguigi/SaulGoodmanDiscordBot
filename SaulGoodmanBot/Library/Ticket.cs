using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public enum ETicketType {
    Feature = 0,
    Issue = 1
}

public enum ETicketStatus {
    New = 0,
    InProgress = 1,
    Closed = 2
}

public class Ticket : DbBase {
    #region Properties
    public int Id { get; private set; }
    public DiscordUser SubmittedBy { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime DateSubmitted { get; set; }
    public ETicketType Type { get; set; }
    public ETicketStatus Status { get; set; }
    public List<DiscordUser> Votes { get; set; } = new();
    public List<TicketAction> Actions { get; set; } = new();
    #endregion
}
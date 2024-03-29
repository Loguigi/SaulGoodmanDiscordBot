namespace SaulGoodmanBot.Data;

public record Constants {
    public static DateTime DATE_ERROR { get; private set; } = DateTime.Parse("1/1/1900");
}

public struct StoredProcedures {
    public const string WHEELS_GETDATA = "Wheels_GetData";
    public const string WHEELS_PROCESS = "Wheels_Process";
    public const string TICKETS_TOGGLE_VOTE = "Ticket_ToggleVote";
}

public enum ETicketType {
    Feature = 0,
    Issue = 1,
    Change = 2
}

public enum ETicketStatus {
    New = 0,
    InProgress = 1,
    Deploy = 2,
    Closed = 3
}
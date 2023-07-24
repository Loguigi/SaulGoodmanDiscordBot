namespace DataLibrary.Models;

public class BirthdayModel {
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public DateOnly Birthday { get; set; }
}
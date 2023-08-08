namespace DataLibrary.Models;

public class LevelModel {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public DateTime MsgLastSent { get; set; }
}
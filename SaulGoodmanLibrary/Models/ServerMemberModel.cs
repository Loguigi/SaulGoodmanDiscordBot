namespace SaulGoodmanLibrary.Models;

public class ServerMemberModel
{
    public long UserId { get; set; }
    public long GuildId { get; set; }
    public string? Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public DateTime MsgLastSent { get; set; }
    public DateTime Birthday { get; set; }
    
    public ServerMemberModel() {}

    public ServerMemberModel(ServerMember member)
    {
        UserId = (long)member.User.Id;
        GuildId = (long)member.Guild.Id;
        Name = member.Name;
        Level = member.Level;
        Experience = member.Experience;
        MsgLastSent = member.MsgLastSent;
        Birthday = member.Birthday;
    }
}
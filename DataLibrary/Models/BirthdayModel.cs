namespace DataLibrary.Models;

public class BirthdayModel {
    public BirthdayModel() {
        GuildId = 0;
        UserId = 0;
        Birthday = DateTime.Now;
    }

    public BirthdayModel(ulong guildid, ulong userid, DateTime birthday) {
        GuildId = (long)guildid;
        UserId = (long)userid;
        Birthday = birthday;
    }
    
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public DateTime Birthday { get; set; }
}
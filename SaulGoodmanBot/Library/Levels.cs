using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class Levels {
    public Levels(DiscordGuild guild, DiscordUser user) {
        Guild = guild;
        User = user;

        var data = LevelProcessor.LoadUser(Guild.Id, User.Id);
        if (data.Count() == 0) {
            LevelProcessor.SaveNewUser(Guild.Id, User.Id, DateTime.Now);
        } else {
            Level = data.First().Level;
            Experience = data.First().Experience;
            MsgLastSent = data.First().MsgLastSent;
        }
    }

    private DiscordGuild Guild { get; set; }
    private DiscordUser User { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public DateTime MsgLastSent { get; set; }
}

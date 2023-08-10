using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class Levels {
    public Levels(DiscordGuild guild, DiscordUser user, DateTime newmsgsent) {
        Guild = guild;
        User = user;
        NewMsgSent = newmsgsent;

        var data = LevelProcessor.LoadUser(Guild.Id, User.Id);
        if (data.Count() == 0) {
            LevelProcessor.SaveNewUser(Guild.Id, User.Id, NewMsgSent);
            MsgLastSent = NewMsgSent.AddDays(-1);
        } else {
            Level = data.First().Level;
            Experience = data.First().Experience;
            MsgLastSent = data.First().MsgLastSent;
        }
    }

    public void GrantExp() {
        Experience += EXP_GAIN;
        int newLevel = (int)Math.Sqrt((Experience/2) + 1);
        if (newLevel > Level) {
            Level++;
            LevelledUp = true;
        }

        LevelProcessor.UpdateExp(Guild.Id, User.Id, Level, Experience, NewMsgSent);
    }

    private DiscordGuild Guild { get; set; }
    private DiscordUser User { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public DateTime MsgLastSent { get; set; }
    public DateTime NewMsgSent { get; set; }
    public bool LevelledUp { get; set; } = false;
    private const int EXP_GAIN = 1;
}

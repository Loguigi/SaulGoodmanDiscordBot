using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class Birthdays {
    public Birthdays(InteractionContext ctx) {
        Context = ctx;
        // TODO: load from database
    }

    public void AddBirthday(Birthday bday) {
        BirthdayProcessor.AddBirthday(Context.Guild.Id, bday.User.Id, bday.BDay);
    }

    private InteractionContext Context { get; set; }
    private List<Birthday> BirthdayList = new List<Birthday>();
}

public class Birthday {
    public Birthday(DiscordUser user, DateOnly bday) {
        User = user;
        BDay = bday;
    }

    public DiscordUser User { get; set; }
    public DateOnly BDay {get; set; }
}

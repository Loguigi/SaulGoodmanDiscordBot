using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class Birthdays {
    // constructor for slash command usage
    public Birthdays(ulong guildid, InteractionContext ctx) {
        GuildId = guildid;
        Context = ctx;
        Client = null;

        var data = BirthdayProcessor.LoadBirthdays(GuildId);
        foreach (var row in data) {
            var _ = GetUsers(row.UserId, row.Birthday);
        }
    }

    // constructor for client handler
    public Birthdays(ulong guildid, DiscordClient client) {
        GuildId = guildid;
        Client = client;
        Context = null;

        var data = BirthdayProcessor.LoadBirthdays(GuildId);
        foreach (var row in data) {
            var _ = GetUsers(row.UserId, row.Birthday);
        }
    }

    public void Add(Birthday bday) {
        // adds birthday to database
        BirthdayProcessor.AddBirthday(GuildId, bday.User.Id, bday.BDay);
    }

    public void Update(Birthday bday) {
        
        BirthdayProcessor.UpdateBirthday(GuildId, bday.User.Id, bday.BDay);
    }

    public async Task GetUsers(ulong userid, DateTime bday) {
        if (Context != null) {
            var user = await Context.Client.GetUserAsync(userid);
            BirthdayList.Add(new Birthday(user, bday));
        } else if (Client != null) {
            var user = await Client.GetUserAsync(userid);
            BirthdayList.Add(new Birthday(user, bday));
        }
    }

    public DateTime Find(DiscordUser user) {
        var bd = BirthdayList.Where(x => x.User == user).FirstOrDefault();
        if (bd == null) {
            return DATE_ERROR;
        } else {
            return bd.BDay;
        }
    }

    public Birthday Next() {
        var nextBirthdays = BirthdayList;

        // change birthday years to next birthday
        foreach (var birthday in nextBirthdays) {
            birthday.BDay = birthday.BDay.AddYears(birthday.GetAge() + 1);
        }

        // sort to find next birthday
        nextBirthdays.Sort((d1, d2) => DateTime.Compare(d1.BDay, d2.BDay));

        return nextBirthdays.First();
    }

    public List<Birthday> GetBirthdays() {
        return BirthdayList;
    }

    private ulong GuildId { get; set; }
    private DiscordClient? Client { get; set; }
    private InteractionContext? Context { get; set; }
    private List<Birthday> BirthdayList = new List<Birthday>();
    public DateTime DATE_ERROR { get; private set; } = DateTime.Parse("1/1/1000");
}

public class Birthday {
    public Birthday(DiscordUser user, DateTime bday) {
        User = user;
        BDay = bday;
    }

    public int GetAge() {
        var current = DateTime.Now;
        var age = current.Year - BDay.Year;

        if (current.Month == BDay.Month) {
            // birthday is within month
            if (current.Day < BDay.Day) {
                // birthday hasn't occured yet
                age--;
            }
        } else if (current.Month < BDay.Month) {
            // birthday month not reached yet
            age--;
        }

        return age;
    }

    public bool IsBirthdayToday() {
        return BDay.Month == DateTime.Now.Month && BDay.Day == DateTime.Now.Day;
    }

    public DiscordUser User { get; set; }
    public DateTime BDay {get; set; }
}

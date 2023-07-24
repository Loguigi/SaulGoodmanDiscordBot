using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class Birthdays {
    public Birthdays(InteractionContext ctx) {
        Context = ctx;
        var data = BirthdayProcessor.LoadBirthdays(Context.Guild.Id);
        foreach (var row in data) {
            var _ = GetUsers(row.UserId, row.Birthday);
        }
    }

    public void Add(Birthday bday) {
        // adds birthday to database
        BirthdayProcessor.AddBirthday(Context.Guild.Id, bday.User.Id, bday.BDay);
    }

    public void Update(Birthday bday) {
        
        BirthdayProcessor.UpdateBirthday(Context.Guild.Id, bday.User.Id, bday.BDay);
    }

    public async Task GetUsers(ulong userid, DateTime bday) {
        var user = await Context.Client.GetUserAsync(userid);
        BirthdayList.Add(new Birthday(user, bday));
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

    private InteractionContext Context { get; set; }
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

    public DiscordUser User { get; set; }
    public DateTime BDay {get; set; }
}

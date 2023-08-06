using DSharpPlus;
using DSharpPlus.Entities;
using DataLibrary.Logic;

namespace SaulGoodmanBot.Library;

public class Birthdays {
    public Birthdays(DiscordGuild guild, DiscordClient client) {
        Guild = guild;
        Client = client;

        var data = BirthdayProcessor.LoadBirthdays(Guild.Id);
        foreach (var row in data) {
            _ = GetUsers((ulong)row.UserId, row.Birthday);
        }
    }

    public void Add(Birthday bday) {
        BirthdayProcessor.AddBirthday(Guild.Id, bday.User.Id, bday.BDay);
    }

    public void Update(Birthday bday) {
        BirthdayProcessor.UpdateBirthday(Guild.Id, bday.User.Id, bday.BDay);
    }

    public async Task GetUsers(ulong userid, DateTime bday) {
        var user = await Client.GetUserAsync(userid);
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

    public bool IsEmpty() {
        return BirthdayList.Count == 0;
    }

    private DiscordGuild Guild { get; set; }
    private DiscordClient Client { get; set; }
    public List<Birthday> BirthdayList { get; private set; } = new();
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

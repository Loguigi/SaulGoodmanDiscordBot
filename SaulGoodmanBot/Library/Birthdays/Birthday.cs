using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library.Birthdays;

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

    public bool HasUpcomingBirthday() {
        return BDay.Month == DateTime.Now.Month && BDay.Day == DateTime.Now.Day + 5;
    }

    public bool HasNoBirthday() {
        return BDay == ServerBirthdays.NO_BIRTHDAY;
    }

    public override string ToString() {
        return BDay.ToString("MMMM d, yyyy");
    }

    public DiscordUser User { get; set; }
    public DateTime BDay {get; set; }
}
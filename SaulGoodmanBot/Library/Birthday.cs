using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

public class Birthday {
    public Birthday(DiscordUser user, DateTime bday) {
        User = user;
        BDay = bday;
    }

    public Birthday(Birthday bday) {
        User = bday.User;
        BDay = bday.BDay;
    }

    public override string ToString() {
        return BDay.ToString("MMMM d, yyyy");
    }

    public DiscordUser User { get; private set; }
    public DateTime BDay { get; set; }
    public int Age {
        get {
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
    }

    public bool HasBirthdayToday {
        get => BDay.Month == DateTime.Now.Month && BDay.Day == DateTime.Now.Day;
    }

    public bool HasUpcomingBirthday {
        get => BDay.Month == DateTime.Now.Month && BDay.Day == DateTime.Now.AddDays(5).Day;
    }
}
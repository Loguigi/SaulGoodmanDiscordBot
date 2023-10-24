using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;

namespace SaulGoodmanBot.Library.Birthdays;

public class ServerBirthdays {
    public ServerBirthdays(DiscordGuild guild) {
        Guild = guild;
        var data = BirthdayProcessor.LoadBirthdays(Guild.Id);
        foreach (var row in data) {
            _ = GetUsers((ulong)row.UserId, row.Birthday);
        }
    }

    public async Task GetUsers(ulong userid, DateTime bday) {
        var user = await Guild.GetMemberAsync(userid);
        Birthdays.Add(new Birthday(user, bday));
    }

    public List<Birthday> GetBirthdays() {
        return Birthdays;
    }

    public int Edit(DataOperations operation, Birthday birthday) => operation switch {
        DataOperations.Add => BirthdayProcessor.AddBirthday(new BirthdayModel(Guild.Id, birthday.User.Id, birthday.BDay)),
        DataOperations.Update => BirthdayProcessor.UpdateBirthday(new BirthdayModel(Guild.Id, birthday.User.Id, birthday.BDay)),
        DataOperations.Delete => BirthdayProcessor.RemoveBirthday(new BirthdayModel(Guild.Id, birthday.User.Id, birthday.BDay)),
        _ => -1
    };

    public Birthday Find(DiscordUser user) {
        return Birthdays.Where(x => x.User == user).FirstOrDefault() ?? new Birthday(user, NO_BIRTHDAY);
    }

    public Birthday Next() {
        var nextBirthdays = Birthdays;

        // change birthday years to next birthday
        foreach (var birthday in nextBirthdays) {
            birthday.BDay = birthday.BDay.AddYears(birthday.GetAge() + 1);
        }

        // sort to find next birthday
        nextBirthdays.Sort((d1, d2) => DateTime.Compare(d1.BDay, d2.BDay));

        return nextBirthdays.First();
    }

    public bool IsEmpty() {
        return Birthdays.Count == 0;
    }

    private DiscordGuild Guild { get; set; }
    private List<Birthday> Birthdays { get; set; } = new();
    public static readonly DateTime NO_BIRTHDAY = DateTime.Parse("1/1/1800");
}

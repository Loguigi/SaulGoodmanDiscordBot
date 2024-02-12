using DSharpPlus.Entities;
using System.Collections;
using System.Data;
using System.Reflection;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Data;
using Dapper;
using DSharpPlus;

namespace SaulGoodmanBot.Controllers;

public class ServerBirthdays : DbBase<BirthdayModel, Birthday>, IEnumerable<Birthday> {
    #region Properties
    private DiscordClient Client { get; set; }
    private DiscordGuild Guild { get; set; }
    public List<Birthday> Birthdays { get; private set; } = new();
    public bool IsEmpty { get => Birthdays.Count == 0; }
    public Birthday Next {
        get {
            var nextBirthdays = Birthdays;

            // change birthday years to next birthday
            foreach (var birthday in nextBirthdays) {
                birthday.BDay = birthday.BDay.AddYears(birthday.Age + 1);
            }

            // sort to find next birthday
            nextBirthdays.Sort((d1, d2) => DateTime.Compare(d1.BDay, d2.BDay));

            return nextBirthdays.FirstOrDefault() ?? throw new Exception($"No birthdays in {Guild.Name}");
        }
    }
    #endregion
    
    #region Public Methods
    public ServerBirthdays(DiscordClient client, DiscordGuild guild) {
        Client = client;
        Guild = guild;
        
        try {
            var result = GetData("Birthdays_GetData", new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            Birthdays = MapData(result.Result!);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public Birthday? this[DiscordUser user] { get => Birthdays.Where(x => x.User == user).FirstOrDefault(); }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Birthday> GetEnumerator() => Birthdays.GetEnumerator();

    public void Save(Birthday bday) {
        try {
            var result = SaveData("Birthdays_Process", new DynamicParameters(
                new BirthdayModel() {
                    GuildId = (long)Guild.Id,
                    UserId = (long)bday.User.Id,
                    Birthday = bday.BDay,
                    Mode = (int)DataMode.SAVE
            })).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Remove(Birthday bday) {
        try {
            var result = SaveData("Birthdays_Process", new DynamicParameters(
                new BirthdayModel() {
                    GuildId = (long)Guild.Id,
                    UserId = (long)bday.User.Id,
                    Birthday = bday.BDay,
                    Mode = (int)DataMode.REMOVE
            })).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override List<Birthday> MapData(List<BirthdayModel> data)
    {
        var birthdays = new List<Birthday>();
        foreach (var b in data) {
            birthdays.Add(new Birthday(GetUser(Client, (ulong)b.UserId).Result, b.Birthday));
        }

        return birthdays;
    }

    private enum DataMode {
        SAVE = 0,
        REMOVE = 1
    }
    #endregion
}

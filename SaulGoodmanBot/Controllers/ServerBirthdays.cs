using DSharpPlus.Entities;
using System.Collections;
using System.Data;
using System.Reflection;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Data;
using Dapper;

namespace SaulGoodmanBot.Controllers;

public class ServerBirthdays : DbBase<BirthdayModel, Birthday>, IEnumerable<Birthday> {
    public ServerBirthdays(DiscordGuild guild) {
        Guild = guild;
        
        try {
            var result = GetData("");
            if (result.Status != ResultArgs<List<BirthdayModel>>.StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            Birthdays = MapData(result.Result);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    #region Public Methods
    public Birthday this[DiscordUser user] {
        get => Birthdays.Where(x => x.User == user).FirstOrDefault()!;
        set {
            try {
                if (this[value.User] == null)
                    Add(value);
                else
                    Change(value);
            } catch (Exception ex) {
                ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
                throw;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Birthday> GetEnumerator() => Birthdays.GetEnumerator();

    public void Add(Birthday bday) {
        try {
            var result = SaveData("", new BirthdayModel() {
                GuildId = (long)Guild.Id,
                UserId = (long)bday.User.Id,
                Birthday = bday.BDay,
                Mode = (int)DataMode.ADD
            });
            if (result.Status != ResultArgs<int>.StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Change(Birthday bday) {
        try {
            var result = SaveData("", new BirthdayModel() {
                GuildId = (long)Guild.Id,
                UserId = (long)bday.User.Id,
                Birthday = bday.BDay,
                Mode = (int)DataMode.CHANGE
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Remove(Birthday bday) {
        try {
            var result = SaveData("", new BirthdayModel() {
                GuildId = (long)Guild.Id,
                UserId = (long)bday.User.Id,
                Birthday = bday.BDay,
                Mode = (int)DataMode.REMOVE
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override ResultArgs<List<BirthdayModel>> GetData(string sp)
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + " @GuildId, @Status, @ErrMsg";
            var param = new BirthdayModel() { GuildId = (long)Guild.Id };
            var data = cnn.Query<BirthdayModel>(sql, param).ToList();

            return new ResultArgs<List<BirthdayModel>>(data, param.Status, param.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override ResultArgs<int> SaveData(string sp, BirthdayModel data)
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + @" @GuildId,
                @UserId,
                @Birthday,
                @Mode,
                @Status,
                @ErrMsg";
            var result = cnn.Execute(sql, data);

            return new ResultArgs<int>(result, data.Status, data.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override List<Birthday> MapData(List<BirthdayModel> data)
    {
        var birthdays = new List<Birthday>();
        foreach (var b in data) {
            birthdays.Add(new Birthday(GetUser((ulong)b.UserId).Result, b.Birthday));
        }

        return birthdays;
    }

    private enum DataMode {
        ADD,
        CHANGE,
        REMOVE
    }

    private async Task<DiscordUser> GetUser(ulong userid) => await Guild.GetMemberAsync(userid);
    #endregion

    #region Properties
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
}

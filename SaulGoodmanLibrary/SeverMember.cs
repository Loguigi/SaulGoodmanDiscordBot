using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class ServerMember : DataAccess
{
    #region Properties
    public DiscordUser User { get; set; }
    public DiscordGuild Guild { get; set; }
    public string? Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Rank { get; set; }
    public int ExpNeededForNextLevel => 2 * (int)Math.Pow(Level + 1, 2) - 2;
    public DateTime MsgLastSent { get; set; }
    public DateTime Birthday { get; set; }
    public string BirthdayToString => Birthday.ToString("MMMM d, yyyy");
    public int Age 
    {
        get 
        {
            var current = DateTime.Now;
            var age = current.Year - Birthday.Year;

            if (current.Month == Birthday.Month) 
            {
                // birthday is within month
                if (current.Day < Birthday.Day) 
                {
                    // birthday hasn't occured yet
                    age--;
                }
            } 
            else if (current.Month < Birthday.Month) 
            {
                // birthday month not reached yet
                age--;
            }

            return age;
        }
    }

    public DateTime NextBirthday => Birthday.AddYears(Age + 1);
    public bool HasBirthdayToday => Birthday.Month == DateTime.Now.Month && Birthday.Day == DateTime.Now.Day;
    public bool HasUpcomingBirthday => NextBirthday.Date <= DateTime.Now.AddDays(5);
    public TimeSpan DaysUntilBirthday => NextBirthday.Date - DateTime.Today;
    #endregion Properties
    
    #region Constructors

    public ServerMember(DiscordGuild guild, DiscordUser user)
    {
        User = user;
        Guild = guild;
        Load();
    }

    public ServerMember(DiscordGuild guild, ServerMemberModel model)
    {
        Guild = guild;
        User = DiscordHelper.GetUser(model.UserId);
        Name = model.Name;
        Level = model.Level;
        Experience = model.Experience;
        MsgLastSent = model.MsgLastSent;
        Birthday = model.Birthday;
    }
    #endregion Constructors
    
    #region Methods

    public sealed override void Load()
    {
        var member = GetData<ServerMemberModel>(StoredProcedures.MEMBER_LOAD_ONE, new DynamicParameters(new { UserId = (long)User.Id, GuildId = (long)Guild.Id })).Result.First();
        Name = member.Name;
        Level = member.Level;
        Experience = member.Experience;
        MsgLastSent = member.MsgLastSent;
        Birthday = member.Birthday;
    }

    public async Task Save() => await SaveData(StoredProcedures.MEMBER_SAVE, new DynamicParameters(new ServerMemberModel(this)));

    public async Task Activate() => await SaveData(StoredProcedures.MEMBER_ACTIVATE, new DynamicParameters(new { UserId = (long)User.Id, GuildId = (long)Guild.Id }));
    
    public async Task Deactivate() => await SaveData(StoredProcedures.MEMBER_DEACTIVATE, new DynamicParameters(new { UserId = (long)User.Id, GuildId = (long)Guild.Id }));

    #endregion Methods
}
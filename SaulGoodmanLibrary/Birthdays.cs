using DSharpPlus.Entities;
using System.Reflection;
using Dapper;
using DSharpPlus;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class Birthdays : DataAccess 
{
    #region Properties
    public DiscordGuild Guild { get; }
    public List<ServerMember> Members => _members.Where(x => x.Birthday != DateTime.Parse("1/1/1900")).ToList();

    public ServerMember Next 
    {
        get 
        {
            var nextBirthdays = Members;

            // sort to find next birthday
            nextBirthdays.Sort((d1, d2) => DateTime.Compare(d1.NextBirthday, d2.NextBirthday));

            return nextBirthdays.FirstOrDefault() ?? throw new Exception($"No birthdays in {Guild.Name}");
        }
    }
    #endregion
    
    #region Public Methods
    public Birthdays(DiscordGuild guild) 
    {
        Guild = guild;
        Load();
    }

    public sealed override void Load()
    {
        _members = [];
        List<ServerMemberModel> data = GetData<ServerMemberModel>(StoredProcedures.MEMBER_LOAD_ALL, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;

        foreach (var member in data)
        {
            _members.Add(new ServerMember(Guild, member));
        }
    }

    public ServerMember? this[DiscordUser user] => _members.FirstOrDefault(b => b.User.Id == user.Id);

    public async Task Save(DiscordUser user, DateTime birthday)
    {
        var member = this[user];

        if (member is not null)
        {
            member.Birthday = birthday;
            await member.Save();
        }
    }
    
    public InteractivityHelper<ServerMember> GetInteractivity(string page = "1") => 
        new(Members, IDHelper.Birthdays.LIST, page, 10, "There are no birthdays");
    #endregion

    private List<ServerMember> _members = [];
}

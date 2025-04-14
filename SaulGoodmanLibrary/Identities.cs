using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class Identities : DataAccess
{
    public DiscordGuild Guild { get; set; }
    public List<ServerMember> Members { get; set; } = [];

    public Identities(DiscordGuild guild)
    {
        Guild = guild;
        Load();
    }
    
    public sealed override void Load()
    {
        Members = [];
        var members = GetData<ServerMemberModel>(StoredProcedures.MEMBER_LOAD_ALL, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;
        foreach (var member in members)
        {
            Members.Add(new ServerMember(Guild, member));
        }
    }

    public async Task ChangeName(ServerMember member, string newName)
    {
        member.Name = newName;
        await SaveData(StoredProcedures.MEMBER_SAVE, new DynamicParameters(new ServerMemberModel(member)));
    }
    
    public string? GetName(DiscordUser user) => Members.First(x => x.User.Id == user.Id).Name;
    
    public InteractivityHelper<ServerMember> GetInteractivity(string page = "1") => 
        new(Members.Where(x => x.Name is not null).ToList(), IDHelper.Misc.WHO, page, 10, "There are no birthdays");
}
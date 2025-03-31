using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class Levels : DataAccess
{
    public DiscordGuild Guild { get; }
    public List<ServerMember> Members { get; private set; } = [];

    public Levels(DiscordGuild guild)
    {
        Guild = guild;
        Load();
    }

    public sealed override void Load()
    {
        Members = [];
        var data = GetData<ServerMemberModel>(StoredProcedures.MEMBER_LOAD_ALL, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result;

        foreach (var member in data)
        {
            Members.Add(new ServerMember(Guild, member));
        }
        
        // Sort list by level and experience
        Members = Members.OrderByDescending(x => x.Level).ThenByDescending(y => y.Experience).ToList();

        // Assign list places to their level ranks
        for (var i = 0; i < Members.Count; i++)
        {
            Members[i].Rank = i + 1;
        }
    }
    
    /// <summary>
    /// Gets a user's level and experience data, or creates a new object to save if no data exists
    /// </summary>
    /// <param name="user">The DiscordUser to search for</param>
    public ServerMember this[DiscordUser user] => Members.FirstOrDefault(x => x.User == user) ?? new ServerMember(Guild, user);

    /// <summary>
    /// Grants an experience point to the user and saves
    /// </summary>
    /// <param name="user"></param>
    /// <returns>True if the user levelled up</returns>
    public async Task<bool> GrantExp(ServerMember user)
    {
        bool leveledUp = false;
        user.MsgLastSent = DateTime.Now;
        
        user.Experience += EXP_GAIN;
        int newLevel = (int)Math.Sqrt((double)user.Experience / 2 + 1);
        if (newLevel > user.Level) 
        {
            user.Level++;
            leveledUp = true;
        }

        await user.Save();
        Load();

        return leveledUp;
    }

    public InteractivityHelper<ServerMember> GetInteractivity(string page = "1") =>
        new(Members, IDHelper.Levels.LEADERBOARD, page, 10);

    private const int EXP_GAIN = 1;
}

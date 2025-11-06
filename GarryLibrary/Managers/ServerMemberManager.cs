using DSharpPlus;
using DSharpPlus.Entities;
using GarryLibrary.Data;
using GarryLibrary.Models;
using Microsoft.Extensions.Logging;

namespace GarryLibrary.Managers;

public class ServerMemberManager(
    IDataRepository<ServerMember> serverMemberRepository,
    DiscordClient discordClient,
    ILogger<ServerMemberManager> logger)
{
    private const int EXP_GAIN = 1;
    
    public async Task<ServerMember> GetMember(DiscordUser user, DiscordGuild guild)
    {
        try
        {
            var members = 
                await serverMemberRepository.FindAsync(x => x.GuildId == (long)guild.Id && x.UserId == (long)user.Id);
            var member = members.FirstOrDefault();

            if (member == null)
            {
                logger.LogInformation("Creating new member record for {User} in guild {Guild}", 
                    user.Username, guild.Name);
                
                member = new ServerMember()
                {
                    GuildId = (long)guild.Id,
                    UserId = (long)user.Id,
                    Level = 1,
                    Experience = 0,
                };
                await serverMemberRepository.CreateAsync(member);
            }
            member.User = user;
            member.Guild = guild;

            return member;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting member {User} in guild {Guild}", user.Username, guild.Name);
            throw;
        }
    }
    
    public async Task<List<ServerMember>> GetMembersAsync(DiscordGuild guild)
    {
        try
        {
            logger.LogDebug("Fetching all members for guild {Guild}", guild.Name);
            
            var members = await serverMemberRepository.FindAsync(x => x.GuildId == (long)guild.Id);
            
            foreach (var member in members)
            {
                member.User = await GetUserAsync(member.UserId);
                member.Guild = guild;
            }

            foreach (var user in guild.Members.Values)
            {
                if (user.IsBot) continue;
                
                var member = members.FirstOrDefault(x => x.UserId == (long)user.Id);

                if (member != null)
                {
                    if (member.Active) continue;
                    
                    member.Active = true;
                    await UpdateMemberAsync(member);
                }
                else
                {
                    var newMember = new ServerMember()
                    {
                        UserId = (long)user.Id,
                        GuildId = (long)guild.Id
                    };

                    await CreateMemberAsync(newMember);
                }
            }
            
            members = members.Where(x => x.Active).ToList();
            members = members.OrderByDescending(x => x.Level).ThenByDescending(y => y.Experience).ToList();

            for (var i = 0; i < members.Count; i++)
            {
                members[i].Rank = i + 1;
            }
            
            logger.LogDebug("Found {Count} members in guild {Guild}", members.Count, guild.Name);
            return members;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching members for guild {Guild}", guild.Name);
            throw;
        }
    }
    
    public async Task CreateMemberAsync(ServerMember member)
    {
        logger.LogInformation("Creating member {UserId} in guild {GuildId}", member.UserId, member.GuildId);
        await serverMemberRepository.CreateAsync(member);
    }
    
    public async Task UpdateMemberAsync(ServerMember member)
    {
        logger.LogDebug("Updating member {UserId} in guild {GuildId}", member.UserId, member.GuildId);
        await serverMemberRepository.UpdateAsync(member);
    }

    public async Task<bool> GrantExp(ServerMember member)
    {
        bool leveledUp = false;

        member.Experience += EXP_GAIN;
        int newLevel = (int)Math.Sqrt((double)member.Experience / 2 + 1);
        if (newLevel > member.Level) 
        {
            member.Level++;
            leveledUp = true;
            logger.LogDebug("Member {UserId} leveled up to {Level}", member.UserId, member.Level);
        }
        await serverMemberRepository.UpdateAsync(member);
        
        return leveledUp;
    }

    public ServerMember GetNextBirthday(List<ServerMember> members)
    {
        members = members.Where(x => x.NextBirthday.HasValue).OrderBy(x => x.NextBirthday).ToList();
        var nextBirthday = members.First();
        logger.LogDebug("Next birthday is {User} on {Date}", nextBirthday.UserId, nextBirthday.NextBirthday);
        return nextBirthday;
    }
    
    private async Task<DiscordUser> GetUserAsync(long userId) => await discordClient.GetUserAsync((ulong)userId);
}
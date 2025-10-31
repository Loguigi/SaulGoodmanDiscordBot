using DSharpPlus;
using DSharpPlus.Entities;
using GarryLibrary.Data;
using GarryLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GarryLibrary.Managers;

public class SecretSantaManager(
    IDataRepository<SantaParticipant> participantRepository,
    IDataRepository<SantaConfig> configRepository,
    DiscordClient discordClient,
    Random random)
{
    public async Task StartEvent(SantaConfig config) => await configRepository.CreateAsync(config);

    public async Task EndEvent(DiscordGuild guild)
    {
        // First get all ServerMember IDs for this guild
        var members = await participantRepository.GetAllAsync(q => q
            .Include(sp => sp.ServerMember));
    
        var guildMemberIds = members
            .Where(x => x.ServerMember.GuildId == (long)guild.Id)
            .Select(x => x.ServerMemberId)
            .ToHashSet();
    
        // Delete participants that match those IDs
        await participantRepository.DeleteAllAsync(x => guildMemberIds.Contains(x.ServerMemberId));
        await configRepository.DeleteAllAsync(x => x.GuildId == (long)guild.Id);
    }

    public async Task AssignNames(List<SantaParticipant> participants, SantaConfig config)
    {
        random.Next();
        
        config.LockedIn = true;
        await configRepository.UpdateAsync(config);
        
        var shuffledParticipants = participants.OrderBy(x => random.Next()).ToList();
        var availableGiftees = new List<SantaParticipant>(shuffledParticipants);

        foreach (var participant in shuffledParticipants)
        {
            // Filter out invalid giftees
            var validGiftees = availableGiftees.Where(x => 
                    x.ServerMemberId != participant.ServerMemberId && // Not themselves
                    x.ServerMemberId != participant.SignificantOtherId) // Not their significant other
                .ToList();
    
            if (validGiftees.Count == 0)
            {
                // Handle edge case: no valid giftees left (need to reshuffle)
                throw new InvalidOperationException("Unable to assign giftees with current constraints. Try reshuffling.");
            }
    
            // Assign a random valid giftee
            var selectedGiftee = validGiftees[random.Next(validGiftees.Count)];
            participant.Giftee = selectedGiftee.ServerMember;
    
            // Remove assigned giftee from available pool
            availableGiftees.Remove(selectedGiftee);
    
            await participantRepository.UpdateAsync(participant);
        }
    }
    
    public async Task<List<SantaParticipant>> GetAllAsync(DiscordGuild guild)
    {
        var participants = 
            await participantRepository.GetAllAsync(q => q
                .Include(sp => sp.ServerMember)
                .Include(sp => sp.Giftee!)
                .Include(sp => sp.SignificantOther!));
        
        participants = participants.Where(x => x.ServerMember.GuildId == (long)guild.Id).ToList();
        
        foreach (var participant in participants)
        {
            await PopulateUsersAsync(participant, false);
        }

        return participants;
    }

    public async Task<SantaParticipant?> GetParticipantAsync(DiscordUser user, DiscordGuild guild)
    {
        var participants = 
            await participantRepository.GetAllAsync(q => q
                .Include(sp => sp.ServerMember)
                .Include(sp => sp.Giftee!)
                .Include(sp => sp.SignificantOther!));
        
        var participant = participants.FirstOrDefault(x => x.ServerMember.UserId == (long)user.Id && x.ServerMember.GuildId == (long)guild.Id);
        
        if (participant != null)
        {
            participant.ServerMember.User = user;
            await PopulateUsersAsync(participant, true);
        }
        
        return participant;
    }

    public async Task AddParticipantAsync(ServerMember member)
    {
        var participant = new SantaParticipant
        {
            ServerMember = member,
        };
        
        await participantRepository.CreateAsync(participant);
    }

    public async Task SaveCouple(SantaParticipant user1, SantaParticipant user2)
    {
        user1.SignificantOther = user2.ServerMember;
        user2.SignificantOther = user1.ServerMember;
        
        await participantRepository.UpdateAsync(user1);
        await participantRepository.UpdateAsync(user2);
    }

    public async Task UpdateConfigAsync(SantaConfig config) => await configRepository.UpdateAsync(config);

    private async Task PopulateUsersAsync(SantaParticipant participant, bool skipBaseUser)
    {
        if (!skipBaseUser)
        {
            participant.ServerMember.User = await GetUserAsync(participant.ServerMember.UserId);
        }
            
        if (participant.Giftee != null)
        {
            participant.Giftee.User = await GetUserAsync(participant.Giftee.UserId);
        }
            
        if (participant.SignificantOther != null)
        {
            participant.SignificantOther.User = await GetUserAsync(participant.SignificantOther.UserId);
        }
    }
    
    private async Task<DiscordUser> GetUserAsync(long userId) => await discordClient.GetUserAsync((ulong)userId);
}
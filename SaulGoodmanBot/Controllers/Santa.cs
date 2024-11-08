using DSharpPlus;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Helpers;
using SaulGoodmanBot.Library;
using System.Data;
using Dapper;
using System.Collections;

namespace SaulGoodmanBot.Controllers;

public class Santa : DataAccess 
{
    #region Properties

    public DiscordGuild Guild { get; }
    public SantaConfig Config { get; private set; }
    public List<SantaParticipant> Participants { get; private set; } = [];
    public bool NotEnoughParticipants => Participants.Count < 3;
    #endregion
    
    #region Public Methods

    public Santa(DiscordClient client, DiscordGuild guild)
    {
        _client = client;
        Guild = guild;
        Config = new SantaConfig(guild);
        
        var participants = GetData<SantaParticipantModel>(StoredProcedures.SANTA_GET_PARTICIPANTS, new DynamicParameters(new { GuildId = guild.Id })).Result;
        foreach (var participant in participants)
        {
            Participants.Add(new SantaParticipant
            (
                GetUser(client, participant.UserId).Result,
                Guild,
                participant.FirstName,
                participant.GifteeId is null ? null : GetUser(client, participant.GifteeId.Value).Result,
                participant.SOId is null ? null : GetUser(client, participant.SOId.Value).Result,
                participant.GiftReady == 1
            ));
        }
    }
    
    public SantaParticipant? FindParticipant(DiscordUser user) => Participants.FirstOrDefault(p => p.User == user);
    
    public void StartEvent(SantaConfig config) 
    {
        Config = config;
        Config.Update();
    }

    public async void EndEvent()
    {
        await SaveData(StoredProcedures.SANTA_END_EVENT, new DynamicParameters(new { GuildId = (long)Guild.Id }));
    }

    public async void SaveParticipant(SantaParticipant participant)
    {
        await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(participant)));
    }

    public async void SaveCouple(SantaParticipant user1, SantaParticipant user2) 
    {
        user1.SO = user2.User;
        user2.SO = user1.User;

        await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(user1)));
        await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(user2)));
    }

    public async void AssignNames() 
    {
        Config.LockedIn = true;
        Config.Update();
        var shuffledParticipants = Participants.OrderBy(x => RandomHelper.RNG.Next()).ToList();

        foreach (var p in Participants) 
        {
            var filteredParticipants = shuffledParticipants.Where(x => x.User != p.User).ToList();
            if (p.SO! != null!)
                filteredParticipants = shuffledParticipants.Where(x => x.User != p.User && x.User != p.SO).ToList();

            p.Giftee = filteredParticipants[RandomHelper.RNG.Next(filteredParticipants.Count)].User;
            await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(p)));
            shuffledParticipants.Remove(Participants.First(x => x.User == p.Giftee));
        }
    }
    #endregion
    
    #region Private Members

    private DiscordClient _client;

    #endregion
}
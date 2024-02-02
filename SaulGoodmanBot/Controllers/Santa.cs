using DSharpPlus;
using DSharpPlus.Entities;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Helpers;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Controllers;

public class Santa : DbBase<SantaParticipantModel, SantaParticipant> {
    public Santa(DiscordClient client, DiscordGuild guild) {
        Client = client;
        Guild = guild;
        Config = new SantaConfig(Guild);
        var participants = SecretSantaProcessor.LoadParticipants(Guild.Id);
        foreach (var p in participants) {
            Participants.Add(new SantaParticipant(Client, p.UserId, p.FirstName, SecretSantaProcessor.LoadWishlist(Guild.Id, (ulong)p.UserId), p.GifteeId, p.SOId, p.GiftReady == 1));
        }
    }

    public void StartEvent(SantaConfig config) {
        Config = config;
        Config.Update();
    }

    public void EndEvent() {
        SecretSantaProcessor.EndEvent(Guild.Id);
    }

    public void AddParticipant(DiscordUser participant, string name) {
        Participants.Add(new SantaParticipant(Client, participant, name));
        SecretSantaProcessor.AddParticipant(new SantaParticipantModel() {
            GuildId = (long)Guild.Id,
            UserId = (long)participant.Id,
            FirstName = name,
        });
    }

    public SantaParticipant? Find(DiscordUser user) {
        return Participants.Where(p => p.User == user).FirstOrDefault();
    }

    public void AddCouple(SantaParticipant user1, SantaParticipant user2) {
        user1.SO = user2.User;
        user2.SO = user1.User;
        SecretSantaProcessor.SetCouple(Guild.Id, user1.User.Id, user2.User.Id);
    }

    public void AssignNames() {
        Config.LockedIn = true;
        Config.Update();
        var shuffledParticipants = Participants.OrderBy(x => RandomHelper.RNG.Next()).ToList();

        foreach (var p in Participants) {
            var y = shuffledParticipants.Where(x => x.User != p.User).ToList();
            if (p.SO! != null!)
                y = shuffledParticipants.Where(x => x.User != p.User && x.User != p.SO).ToList();

            p.Giftee = y[RandomHelper.RNG.Next(y.Count)].User;
            SecretSantaProcessor.AssignGiftee(Guild.Id, p.User.Id, p.Giftee.Id);
            shuffledParticipants.Remove(Participants.Where(x => x.User == p.Giftee).First());
        }
    }

    public bool NotEnoughParticipants() {
        return Participants.Count < 3;
    }

    #region DB Methods
    protected override ResultArgs<List<SantaParticipantModel>> GetData(string sp)
    {
        throw new NotImplementedException();
    }

    protected override ResultArgs<int> SaveData(string sp, SantaParticipantModel data)
    {
        throw new NotImplementedException();
    }

    protected override List<SantaParticipant> MapData(List<SantaParticipantModel> data)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Properties
    private DiscordClient Client { get; set; }
    private DiscordGuild Guild { get; set; }
    public SantaConfig Config { get; private set; }
    public List<SantaParticipant> Participants { get; private set; } = new();
    #endregion
}
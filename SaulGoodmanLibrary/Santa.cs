using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanBot.Helpers;
using SaulGoodmanData;
using SaulGoodmanLibrary.Helpers;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class Santa : DataAccess 
{
    #region Properties
    public DiscordGuild Guild { get; }
    public SantaConfig Config { get; private set; }
    public List<SantaParticipant> Participants { get; private set; } = [];
    public bool NotEnoughParticipants => Participants.Count < 3;
    #endregion Properties
    
    #region Public Methods

    public Santa(DiscordGuild guild)
    {
        Guild = guild;
        Config = new SantaConfig(guild);
        Load();
    }

    public sealed override void Load()
    {
        Participants = [];
        var data = GetData<SantaParticipantModel>(StoredProcedures.SANTA_GET_PARTICIPANTS, new DynamicParameters(new { p_GuildId = (long)Guild.Id })).Result;

        foreach (var participant in data)
        {
            Participants.Add(new SantaParticipant(participant));
        }
    }

    public SantaParticipant? FindParticipant(DiscordUser user) => Participants.FirstOrDefault(p => p.User == user);
    
    public async Task StartEvent(SantaConfig config) 
    {
        Config = config;
        await Config.Update();
    }

    public async Task EndEvent()
    {
        await SaveData(StoredProcedures.SANTA_END_EVENT, new DynamicParameters(new { p_GuildId = (long)Guild.Id }));
        Load();
    }

    public async Task SaveParticipant(SantaParticipant participant)
    {
        await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(Guild.Id, participant)));
    }

    public async Task SaveCouple(SantaParticipant user1, SantaParticipant user2) 
    {
        user1.SO = user2.User;
        user2.SO = user1.User;

        await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(Guild.Id, user1)));
        await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(Guild.Id, user2)));
        Load();
    }

    public async Task AssignNames() 
    {
        Config.LockedIn = true;
        await Config.Update();
        var shuffledParticipants = Participants.OrderBy(x => RandomHelper.RNG.Next()).ToList();

        foreach (var p in Participants) 
        {
            var filteredParticipants = shuffledParticipants.Where(x => x.User != p.User).ToList();
            if (p.SO! != null!)
                filteredParticipants = shuffledParticipants.Where(x => x.User != p.User && x.User != p.SO).ToList();

            p.Giftee = filteredParticipants[RandomHelper.RNG.Next(filteredParticipants.Count)].User;
            await SaveData(StoredProcedures.SANTA_SAVE_PARTICIPANT, new DynamicParameters(new SantaParticipantModel(Guild.Id, p)));
            shuffledParticipants.Remove(Participants.First(x => x.User == p.Giftee));
        }
    }
    #endregion
    
    public class SantaParticipant
    {
        #region Properties
        public DiscordUser User { get; }
        public string FirstName { get; set; }
        public DiscordUser? Giftee { get; set; }
        public DiscordUser? SO { get; set; }
        public bool GiftReady { get; set; }
        #endregion

        public SantaParticipant(DiscordUser user, string firstName)
        {
            User = user;
            FirstName = firstName;
            Giftee = null;
            SO = null;
            GiftReady = false;
        }
        
        public SantaParticipant(SantaParticipantModel participant)
        {
            User = DiscordHelper.GetUser(participant.UserId);
            FirstName = participant.FirstName;
            Giftee = participant.GifteeId is null ? null : DiscordHelper.GetUser(participant.GifteeId.Value);
            SO = participant.SOId is null ? null : DiscordHelper.GetUser(participant.SOId.Value);
            GiftReady = participant.GiftReady == 1;
        }
    }
    
    public class SantaConfig : DataAccess 
    {
        #region Properties
        public DiscordGuild Guild { get; }
        public DiscordRole? SantaRole { get; set; }
        public bool HasStarted => ParticipationDeadline > DateTime.Now || LockedIn;
        public DateTime ParticipationDeadline { get; set; }
        public DateTime ExchangeDate { get; set; }
        public string ExchangeLocation { get; set; } = string.Empty;
        public string? ExchangeAddress { get; set;  }
        public double? PriceLimit { get; set; }
        public bool LockedIn { get; set; }
        #endregion

        #region Public Methods

        public SantaConfig(DiscordGuild guild)
        {
            Guild = guild;
            Load();
        }

        public sealed override void Load()
        {
            var config = GetData<SantaConfigModel>(StoredProcedures.SANTA_GET_CONFIG, new DynamicParameters(new { GuildId = (long)Guild.Id })).Result.First();
            
            if (config.SantaRoleId != 0)
            {
                SantaRole = Guild.GetRole((ulong)config.SantaRoleId);    
            }
        
            ParticipationDeadline = config.ParticipationDeadline;
            ExchangeDate = config.ExchangeDate;
            ExchangeLocation = config.ExchangeLocation;
            ExchangeAddress = config.ExchangeAddress;
            PriceLimit = config.PriceLimit;
            LockedIn = config.LockedIn == 1;
        }

        public async Task Update()
        {
            await SaveData(StoredProcedures.SANTA_SAVE_CONFIG, new DynamicParameters(new SantaConfigModel(this)));
            Load();
        }
        #endregion
    }
}
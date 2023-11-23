using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class SecretSantaProcessor {
    public static int StartEvent(SantaConfigModel config) {
        string sql = @"insert into dbo.SantaConfig values (@GuildId, @ParticipationDeadline, @ExchangeDate, @ExchangeLocation, @PriceLimit, @LockedIn);";
        return SqlDataAccess.SaveData(sql, config);
    }

    public static int EndEvent(ulong guildid) {
        string sql = $"delete from dbo.SantaParticipants where GuildId={guildid}; delete from dbo.SantaConfig where GuildId={guildid}";
        return SqlDataAccess.SaveData(sql, new SantaConfigModel());
    }

    public static int AddParticipant(SantaParticipantModel user) {
        string sql = @"insert into dbo.SantaParticipants values (@GuildId, @UserId, @FirstName, @GifteeId, @SOId);";
        return SqlDataAccess.SaveData(sql, user);
    }

    public static SantaConfigModel? LoadConfig(ulong guildid) {
        string sql = $"select * from dbo.SantaConfig where GuildId={guildid};";
        return SqlDataAccess.LoadData<SantaConfigModel>(sql).FirstOrDefault();
    }

    public static List<SantaParticipantModel> LoadParticipants(ulong guildid) {
        string sql = $"select * from dbo.SantaParticipants where GuildId={guildid};";
        return SqlDataAccess.LoadData<SantaParticipantModel>(sql);
    }

    public static int AssignGiftee(ulong guildid, ulong gifterid, ulong gifteeid) {
        string sql = $"update dbo.SantaParticipants set GifteeId={gifteeid} where UserId={gifterid} and GuildId={guildid}";
        return SqlDataAccess.SaveData(sql, new SantaParticipantModel());
    }

    public static int SetCouple(ulong guildid, ulong user1id, ulong user2id) {
        string sql = $"update dbo.SantaParticipants set SOId={user2id} where UserId={user1id} and GuildId={guildid}; update dbo.SantaParticipants set SOId={user1id} where UserId={user2id} and GuildId={guildid}";
        return SqlDataAccess.SaveData(sql, new SantaParticipantModel());
    }

    public static int UpdateConfig(SantaConfigModel config) {
        string sql = @"update dbo.SantaConfig set ParticipationDeadline=@ParticipationDeadline, ExchangeDate=@ExchangeDate, ExchangeLocation=@ExchangeLocation, PriceLimit=@PriceLimit, LockedIn=@LockedIn where GuildId=@GuildId;";
        return SqlDataAccess.SaveData(sql, config);
    }
}
using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class SecretSantaProcessor {
    public static int StartEvent(SantaConfigModel config) {
        // TODO implement
        return 1;
    }

    public static int AddParticipant(SantaParticipantModel user) {
        string sql = @"insert into dbo.SantaParticipants values (@GuildId, @UserId, @FirstName);";
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
        // TODO finish update
        return 1;
    }
}
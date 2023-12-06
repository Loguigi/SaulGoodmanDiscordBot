using DataLibrary.Config;
using DataLibrary.Models;

namespace DataLibrary.Logic;

public static class SecretSantaProcessor {
    public static int StartEvent(SantaConfigModel config) {
        string sql = @"insert into dbo.SantaConfig values (@GuildId, @SantaRoleId, @ParticipationDeadline, @ExchangeDate, @ExchangeLocation, @ExchangeAddress, @PriceLimit, @LockedIn);";
        return SqlDataAccess.SaveData(sql, config);
    }

    public static int EndEvent(ulong guildid) {
        string sql = $"delete from dbo.SantaParticipants where GuildId={guildid}; delete from dbo.SantaConfig where GuildId={guildid}; delete from dbo.SantaWishlist where GuildId={guildid};";
        return SqlDataAccess.SaveData(sql, new SantaConfigModel());
    }

    public static int AddParticipant(SantaParticipantModel user) {
        string sql = @"insert into dbo.SantaParticipants values (@GuildId, @UserId, @FirstName, @GifteeId, @SOId, @GiftReady);";
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

    public static List<string> LoadWishlist(ulong guildid, ulong userid) {
        string sql = $"select WishlistItem from dbo.SantaWishlist where GuildId={guildid} and UserId={userid};";
        return SqlDataAccess.LoadData<string>(sql);
    }

    public static int AddWishlistItem(SantaWishlistModel wishlist) {
        string sql = @"insert into dbo.SantaWishlist values (@GuildId, @UserId, @WishlistItem);";
        return SqlDataAccess.SaveData(sql, wishlist);
    }

    public static int RemoveWishlistItem(SantaWishlistModel wishlist) {
        string sql = @"delete from dbo.SantaWishlist where GuildId=@GuildId and UserId=@UserId and WishlistItem=@WishlistItem;";
        return SqlDataAccess.SaveData(sql, wishlist);
    }

    public static int AssignGiftee(ulong guildid, ulong gifterid, ulong gifteeid) {
        string sql = $"update dbo.SantaParticipants set GifteeId={gifteeid} where UserId={gifterid} and GuildId={guildid}";
        return SqlDataAccess.SaveData(sql, new SantaParticipantModel());
    }

    public static int SetGiftReady(SantaParticipantModel user) {
        string sql = @"update dbo.SantaParticipants set GiftReady=@GiftReady where GuildId=@GuildId and UserId=@UserId;";
        return SqlDataAccess.SaveData(sql, user);
    }

    public static int SetCouple(ulong guildid, ulong user1id, ulong user2id) {
        string sql = $"update dbo.SantaParticipants set SOId={user2id} where UserId={user1id} and GuildId={guildid}; update dbo.SantaParticipants set SOId={user1id} where UserId={user2id} and GuildId={guildid}";
        return SqlDataAccess.SaveData(sql, new SantaParticipantModel());
    }

    public static int UpdateConfig(SantaConfigModel config) {
        string sql = @"update dbo.SantaConfig set ParticipationDeadline=@ParticipationDeadline, ExchangeDate=@ExchangeDate, ExchangeLocation=@ExchangeLocation, ExchangeAddress=@ExchangeAddress, PriceLimit=@PriceLimit, LockedIn=@LockedIn where GuildId=@GuildId;";
        return SqlDataAccess.SaveData(sql, config);
    }
}
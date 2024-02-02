using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;

namespace SaulGoodmanBot.Library;

public class SantaParticipant : DbBase<SantaWishlistModel, string> {
    public SantaParticipant(DiscordClient client, DiscordUser user, string name, DiscordUser? giftee=null, DiscordUser? SO=null) {
        Client = client;
        User = user;
        FirstName = name;
        Giftee = giftee;
        this.SO = SO;
    }

    public SantaParticipant(DiscordClient client, long userid, string name, List<string> wishlist, long? gifteeid=null, long? SOid=null, bool giftready=false) {
        Client = client;
        User = GetUserData(userid).Result;
        FirstName = name;
        Wishlist = wishlist;
        Giftee = gifteeid != null ? GetUserData(gifteeid ?? 0).Result : null;
        SO = SOid != null ? GetUserData(SOid ?? 0).Result : null;
        GiftReady = giftready;
    }

    private async Task<DiscordUser> GetUserData(long id) {
        return await Client.GetUserAsync((ulong)id);
    }

    public int EditWishlist(ulong guildid, DataOperations operation, string item) => operation switch {
        DataOperations.Add => SecretSantaProcessor.AddWishlistItem(new SantaWishlistModel() {GuildId = (long)guildid, UserId = (long)User.Id, WishlistItem=item}),
        DataOperations.Delete => SecretSantaProcessor.RemoveWishlistItem(new SantaWishlistModel() {GuildId = (long)guildid, UserId = (long)User.Id, WishlistItem=item}),
        _ => -1
    };

    public bool IsWishlistFull() {
        return Wishlist.Count == MAX_WISHLIST_ITEMS;
    }

    public void SetGiftReady(ulong guildid) {
        GiftReady = !GiftReady;
        SecretSantaProcessor.SetGiftReady(new SantaParticipantModel() {
            GuildId = (long)guildid,
            UserId = (long)User.Id,
            GiftReady = GiftReady ? 1 : 0
        });
    }

    #region DB Methods
    protected override ResultArgs<List<SantaWishlistModel>> GetData(string sp)
    {
        throw new NotImplementedException();
    }

    protected override ResultArgs<int> SaveData(string sp, SantaWishlistModel data)
    {
        throw new NotImplementedException();
    }

    protected override List<string> MapData(List<SantaWishlistModel> data)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Properties
    private DiscordClient Client { get; set; }
    public DiscordUser User { get; private set; }
    public string FirstName { get; set; }
    public List<string> Wishlist { get; set; } = new();
    public DiscordUser? Giftee { get; set; }
    public DiscordUser? SO { get; set; }
    public bool GiftReady { get; set; } = false;
    public const int MAX_WISHLIST_ITEMS = 20;
    #endregion
}
// using DSharpPlus;
// using DSharpPlus.Entities;
// using DSharpPlus.EventArgs;
// using SaulGoodmanBot.Data;
// using SaulGoodmanBot.Models;

// namespace SaulGoodmanBot.Library;

// public class SantaParticipant : DbBase<SantaWishlistModel, string> {
//     #region Properties
//     public DiscordUser User { get; private set; }
//     public string FirstName { get; set; } = string.Empty;
//     public List<string> Wishlist { get; set; } = new();
//     public DiscordUser? Giftee { get; set; }
//     public DiscordUser? SO { get; set; }
//     public bool GiftReady { get; set; } = false;
//     public const int MAX_WISHLIST_ITEMS = 20;
//     #endregion

//     public SantaParticipant(DiscordUser user) => User = user;

//     public bool IsWishlistFull() {
//         return Wishlist.Count == MAX_WISHLIST_ITEMS;
//     }

//     public void SetGiftReady(ulong guildid) {
//         GiftReady = !GiftReady;
//         SecretSantaProcessor.SetGiftReady(new SantaParticipantModel() {
//             GuildId = (long)guildid,
//             UserId = (long)User.Id,
//             GiftReady = GiftReady ? 1 : 0
//         });
//     }

//     #region DB Methods
//     protected override ResultArgs<List<SantaWishlistModel>> GetData(string sp)
//     {
//         throw new NotImplementedException();
//     }

//     protected override ResultArgs<int> SaveData(string sp, SantaWishlistModel data)
//     {
//         throw new NotImplementedException();
//     }

//     protected override List<string> MapData(List<SantaWishlistModel> data)
//     {
//         throw new NotImplementedException();
//     }
//     #endregion
// }
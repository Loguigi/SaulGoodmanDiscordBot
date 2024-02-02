using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.Models;

public class SantaWishlistModel : DbCommonParams {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public string WishlistItem { get; set; } = string.Empty;
}
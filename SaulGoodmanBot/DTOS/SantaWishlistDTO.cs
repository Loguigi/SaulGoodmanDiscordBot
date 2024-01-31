using SaulGoodmanBot.Data;

namespace SaulGoodmanBot.DTO;

public class SantaWishlistDTO : DbCommonParams {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public string WishlistItem { get; set; } = string.Empty;
}
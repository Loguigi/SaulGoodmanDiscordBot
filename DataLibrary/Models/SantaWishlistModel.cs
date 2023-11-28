namespace DataLibrary.Models;

public class SantaWishlistModel {
    public long GuildId { get; set; }
    public long UserId { get; set; }
    public string WishlistItem { get; set; } = string.Empty;
}
using DSharpPlus;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library.SecretSanta;

public class SantaParticipant {
    public SantaParticipant(DiscordClient client, DiscordUser user, string name, DiscordUser? giftee=null, DiscordUser? SO=null) {
        Client = client;
        User = user;
        FirstName = name;
        Giftee = giftee;
        this.SO = SO;
    }

    public SantaParticipant(DiscordClient client, long userid, string name, long? gifteeid=null, long? SOid=null) {
        Client = client;
        User = GetUserData(userid).Result;
        FirstName = name;
        Giftee = gifteeid != null ? GetUserData(gifteeid ?? 0).Result : null;
        SO = SOid != null ? GetUserData(SOid ?? 0).Result : null;
    }

    private async Task<DiscordUser> GetUserData(long id) {
        return await Client.GetUserAsync((ulong)id);
    }

    private DiscordClient Client { get; set; }
    public DiscordUser User { get; private set; }
    public string FirstName { get; set; }
    public DiscordUser? Giftee { get; set; }
    public DiscordUser? SO { get; set; }
}
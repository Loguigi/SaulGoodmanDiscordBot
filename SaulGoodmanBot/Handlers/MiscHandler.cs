using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class MiscHandler {
    public static async Task HandleFlip(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Misc.FLIP)) {
            await Task.CompletedTask;
            return;
        }

        var last_flip = e.Id.Split('\\')[LAST_FLIP_INDEX];
        int heads_count = int.Parse(e.Id.Split('\\')[HEADS_COUNT_INDEX]);
        int tails_count = int.Parse(e.Id.Split('\\')[TAILS_COUNT_INDEX]);

        var coin = new Random();
        int flip = coin.Next(1, 3);

        heads_count += flip == 1 ? 1 : 0;
        tails_count += flip != 1 ? 1 : 0;

        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Coin Flip", "", ImageHelper.Images["Coin"])
            .WithDescription($"# {((flip == 1) ? "Heads" : "Tails")}")
            .WithThumbnail(ImageHelper.Images["PS2Jesse"])
            .AddField("Last Flip", last_flip, false)
            .AddField("Heads", heads_count.ToString(), true)
            .AddField("Tails", tails_count.ToString(), true)
            .WithFooter($"Flipped by {e.User.GlobalName}", e.User.AvatarUrl)
            .WithColor((flip == 1) ? DiscordColor.Aquamarine : DiscordColor.Rose);

        var flipButton = new DiscordButtonComponent(ButtonStyle.Success, $"{IDHelper.Misc.FLIP}\\{(flip == 1 ? "Heads" : "Tails")}\\{heads_count}\\{tails_count}", "Flip Again", false);

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(flipButton)));
    }

    private const int LAST_FLIP_INDEX = 1;
    private const int HEADS_COUNT_INDEX = 2;
    private const int TAILS_COUNT_INDEX = 3;
}
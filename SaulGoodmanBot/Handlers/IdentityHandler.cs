using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class IdentityHandler
{
    public static async Task HandleWhoList(DiscordClient s, ComponentInteractionCreateEventArgs e)
    {
        if (!e.Id.Contains(IDHelper.Misc.WHO))
        {
            await Task.CompletedTask;
            return;
        }
        
        var identities = new Identities(e.Guild).GetInteractivity();

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Who are you?")
            .WithColor(DiscordColor.Aquamarine)
            .WithDescription("")
            .WithAuthor(e.Guild.Name, null, e.Guild.IconUrl)
            .WithFooter(identities.PageStatus);

        foreach (var member in identities.GetPage())
        {
            embed.Description += $"### * {member.User.Mention} => {member.Name}\n";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(identities.AddPageButtons().AddEmbed(embed)));
    }
}
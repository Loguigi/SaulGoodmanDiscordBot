using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Commands;

namespace SaulGoodmanBot.Handlers;

public static class MiscHandlers {
    public static async Task HandleFlip(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "flipagain") {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new MiscCommands().CoinFlip()));
        }
    }
}
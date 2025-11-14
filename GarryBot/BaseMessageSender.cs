using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace GarryBot;

public abstract class BaseMessageSender
{
    protected async Task SendMessage(SlashCommandContext ctx, DiscordInteractionResponseBuilder builder, bool ephemeral = false)
    {
        builder.IsEphemeral = ephemeral;
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
    
    protected async Task UpdateMessage(InteractionCreatedEventArgs e, DiscordInteractionResponseBuilder builder)
    {
        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, builder);
    }
}
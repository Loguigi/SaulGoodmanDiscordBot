using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;

namespace GarryBot;

public static class Validation
{
    public static async Task<bool> IsBot(SlashCommandContext ctx)
    {
        if (ctx.User.IsBot)
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateError("This is a bot")).AsEphemeral());
        }
        
        return ctx.User.IsBot;
    }

    public static async Task<bool> IsBot(SlashCommandContext ctx, DiscordUser? user)
    {
        if (user == null) return false;
        
        if (user.IsBot)
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateError("This is a bot")).AsEphemeral());
        }
        
        return user.IsBot;
    }

    public static async Task<bool> BirthdayNotSet(SlashCommandContext ctx, ServerMember member)
    {
        if (!member.Birthday.HasValue)
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateError("Birthday not set")).AsEphemeral());
        }
        
        return !member.Birthday.HasValue;
    }
    
    public static async Task<(bool hasWheels, List<WheelPicker> wheels)> GetWheelsOrError(
        SlashCommandContext ctx, 
        WheelPickerManager wheelManager)
    {
        var wheels = await wheelManager.GetAllAsync(ctx.Guild!);
        
        if (wheels.Count == 0)
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateError("No wheels found")).AsEphemeral());
            return (false, wheels);
        }
        
        return (true, wheels);
    }
}
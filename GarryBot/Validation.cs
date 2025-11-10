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
            await ctx.RespondAsync(MessageTemplates.CreateError("This is a bot"), true);
        }
        
        return ctx.User.IsBot;
    }

    public static async Task<bool> IsBot(SlashCommandContext ctx, DiscordUser? user)
    {
        if (user == null) return false;
        
        if (user.IsBot)
        {
            await ctx.RespondAsync(MessageTemplates.CreateError("This is a bot"), true);
        }
        
        return user.IsBot;
    }

    public static async Task<bool> BirthdayNotSet(SlashCommandContext ctx, ServerMember member)
    {
        if (!member.Birthday.HasValue)
        {
            await ctx.RespondAsync(MessageTemplates.CreateError("This user does not have a birthday set"), true);
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
            await ctx.RespondAsync(MessageTemplates.CreateError("No wheels found in this guild"), true);
            return (false, wheels);
        }
        
        return (true, wheels);
    }
}
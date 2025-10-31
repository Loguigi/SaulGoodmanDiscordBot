using DSharpPlus.Commands.Processors.SlashCommands;
using GarryLibrary.Helpers;
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

    public static async Task<bool> BirthdayNotSet(SlashCommandContext ctx, ServerMember member)
    {
        if (!member.Birthday.HasValue)
        {
            await ctx.RespondAsync(MessageTemplates.CreateError("This user does not have a birthday set"), true);
        }
        
        return !member.Birthday.HasValue;
    }
}
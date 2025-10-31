using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

public class MiscCommands(
    ServerMemberManager memberManager,
    Random random,
    ILogger<MiscCommands> logger)
{
    [Command("who")]
    public async Task Who(SlashCommandContext ctx)
    {
        try
        {
            logger.LogInformation("Who command invoked by {User} in guild {Guild}", 
                ctx.User.Username, ctx.Guild?.Name);
            
            var members = await memberManager.GetMembersAsync(ctx.Guild!);
            // Your logic here
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing who command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred while fetching members"), true);
        }
    }
    
    [Command("egg")]
    public async Task Egg(SlashCommandContext ctx)
    {
        try
        {
            logger.LogInformation("Egg command invoked by {User} in guild {Guild}", 
                ctx.User.Username, ctx.Guild?.Name);
            
            var members = await memberManager.GetMembersAsync(ctx.Guild!);
            await ctx.RespondWithModalAsync(
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateEggCounter(ctx.Guild!, members, "1")));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing egg command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }

    [Command("flip")]
    public async Task Flip(SlashCommandContext ctx)
    {
        try
        {
            logger.LogInformation("Flip command invoked by {User} in guild {Guild}", 
                ctx.User.Username, ctx.Guild?.Name);
            
            var flip = random.Next(2) == 1 ? FlipResult.Heads : FlipResult.Tails;
            var member = await memberManager.GetMember(ctx.User, ctx.Guild!);
            
            await ctx.RespondWithModalAsync(new DiscordInteractionResponseBuilder(
                MessageTemplates.CreateCoinFlip(member, FlipData.FirstFlip(flip))));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing flip command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }

    [Command("rng")]
    public async Task Rng(SlashCommandContext ctx, int min, int max)
    {
        try
        {
            logger.LogInformation("RNG command invoked by {User} with range {Min}-{Max}", 
                ctx.User.Username, min, max);
            
            if (min > max)
            {
                logger.LogWarning("User {User} provided invalid range: min={Min} > max={Max}", 
                    ctx.User.Username, min, max);
                await ctx.RespondAsync(MessageTemplates.CreateError("Min must be less than max"), true);
                return;
            }
            
            int number = random.Next(min, max + 1);
            logger.LogDebug("Generated random number: {Number}", number);
            
            await ctx.RespondAsync(MessageTemplates.CreateRandomNumber(number, min, max));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing RNG command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }
}
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
    : BaseCommand<MiscCommands>(logger)
{
    private readonly ILogger<MiscCommands> _logger = logger;

    [Command("who"), RequireGuild]
    public async Task Who(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await memberManager.GetMembersAsync(ctx.Guild!);

            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(
                    MessageTemplates.CreateIdentityDisplay(ctx.Guild!, members, "1")));
        }, "who");
    }
    
    [Command("egg"), RequireGuild]
    public async Task Egg(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await memberManager.GetMembersAsync(ctx.Guild!);
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateEggCounter(ctx.Guild!, members, "1")));
        }, "egg");
    }

    [Command("flip"), RequireGuild]
    public async Task Flip(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var flip = random.Next(2) == 1 ? FlipResult.Heads : FlipResult.Tails;
            var member = await memberManager.GetMember(ctx.User, ctx.Guild!);

            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(
                    MessageTemplates.CreateCoinFlip(member, FlipData.FirstFlip(flip))));
        }, "flip");
    }

    [Command("rng"), RequireGuild]
    public async Task Rng(SlashCommandContext ctx, int min, int max)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (min > max)
            {
                _logger.LogWarning("User {User} provided invalid range: min={Min} > max={Max}",
                    ctx.User.Username, min, max);
                await ctx.RespondAsync(MessageTemplates.CreateError("Min must be less than max"), true);
                return;
            }

            int number = random.Next(min, max + 1);
            _logger.LogDebug("Generated random number: {Number}", number);

            await ctx.RespondAsync(MessageTemplates.CreateRandomNumber(number, min, max));
        }, "rng");
    }
}
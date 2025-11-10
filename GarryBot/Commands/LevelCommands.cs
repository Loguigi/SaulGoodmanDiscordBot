using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;


public class LevelCommands(
    ServerMemberManager memberManager,
    ILogger<LevelCommands> logger)
    : BaseCommand<LevelCommands>(logger)
{
    [Command("level"), RequireGuild]
    public async Task Level(SlashCommandContext ctx, DiscordUser? user = null)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (await Validation.IsBot(ctx, user)) return;

            var members = await memberManager.GetMembersAsync(ctx.Guild!);
            var member = members.First(x => x.User == (user ?? ctx.User));

            await ctx.RespondAsync(MessageTemplates.CreateLevelCard(member, ctx.Guild!));
        }, "level");
    }

    [Command("leaderboard"), RequireGuild]
    public async Task Leaderboard(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await memberManager.GetMembersAsync(ctx.Guild!);

            await ctx.RespondWithModalAsync(
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateLeaderboard(members, ctx.Guild!, "1")));
        }, "leaderboard");
    }
}
using System.ComponentModel;
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
    [Command("level"), RequireGuild, Description("Displays your level card or another user's level card")]
    public async Task Level(SlashCommandContext ctx, DiscordUser? user = null)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (await Validation.IsBot(ctx, user)) return;

            var members = await memberManager.GetMembersAsync(ctx.Guild!);
            var member = members.First(x => x.User == (user ?? ctx.User));

            await SendMessage(ctx, MessageTemplates.CreateLevelCard(member, ctx.Guild!));
        }, "level");
    }

    [Command("leaderboard"), RequireGuild, Description("Displays the server leaderboard")]
    public async Task Leaderboard(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await memberManager.GetMembersAsync(ctx.Guild!);

            await SendMessage(ctx, MessageTemplates.CreateLeaderboard(members, ctx.Guild!, "1"));
        }, "leaderboard");
    }
}
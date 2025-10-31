using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;

namespace GarryBot.Commands;


public class LevelCommands(
    ServerMemberManager memberManager)
{
    [Command("level"), RequireGuild]
    public async Task Level(SlashCommandContext ctx, DiscordUser? user = null)
    {
        if (user != null && user.IsBot)
        {
            await ctx.RespondAsync(MessageTemplates.CreateError("This is a bot"), true);
            return;
        }

        var member = await memberManager.GetMember(user ?? ctx.User, ctx.Guild!);

        await ctx.RespondAsync(MessageTemplates.CreateLevelCard(member, ctx.Guild!));
    }

    [Command("leaderboard"), RequireGuild]
    public async Task Leaderboard(SlashCommandContext ctx)
    {
        var members = await memberManager.GetMembersAsync(ctx.Guild!);

        await ctx.RespondWithModalAsync(
            new DiscordInteractionResponseBuilder(MessageTemplates.CreateLeaderboard(members, ctx.Guild!, "1")));
    }
}
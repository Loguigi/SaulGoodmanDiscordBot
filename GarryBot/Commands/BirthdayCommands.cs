using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

/// <summary>
/// Commands for managing birthdays
/// - birthday check (user)
/// - birthday next
/// - birthday list
/// - birthday change (user) (year) (month) (day)
/// - birthday add (year) (month) (day)
/// </summary>
/// <param name="memberManager">Server member manager service</param>
[Command("birthday")]
public class BirthdayCommands(
    ServerMemberManager memberManager,
    ILogger<BirthdayCommands> logger)
{
    [Command("check"), RequireGuild]
    public async Task CheckBirthday(SlashCommandContext ctx, DiscordUser user)
    {
        try
        {
            if (await Validation.IsBot(ctx)) return;

            var member = await memberManager.GetMember(user, ctx.Guild!);

            if (await Validation.BirthdayNotSet(ctx, member)) return;

            await ctx.RespondAsync(MessageTemplates.CreateBirthdayCard(member), true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing birthday check command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }

    [Command("next"), RequireGuild]
    public async Task NextBirthday(SlashCommandContext ctx)
    {
        try
        {
            var members = await GetServerMembers(ctx.Guild!);
            var nextBirthday = memberManager.GetNextBirthday(members);

            await ctx.RespondAsync(MessageTemplates.CreateNextBirthday(nextBirthday, ctx.Guild!));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing birthday next command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }

    [Command("list"), RequireGuild]
    public async Task BirthdayList(SlashCommandContext ctx)
    {
        try
        {
            var members = await GetServerMembers(ctx.Guild!);
            var filteredMembers = members.Where(x => x.Birthday.HasValue).ToList();

            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(
                    MessageTemplates.CreateBirthdayList(ctx.Guild!, filteredMembers, "1")));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing birthday list command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }

    [Command("change"), RequireGuild]
    [RequirePermissions(DiscordPermissions.Administrator)]
    public async Task ChangeBirthday(SlashCommandContext ctx, DiscordUser user, long year, long month, long day)
    {
        try
        {
            if (await Validation.IsBot(ctx)) return;

            if (!DateTime.TryParse($"{year}-{month}-{day}", out var date))
            {
                await ctx.RespondAsync(MessageTemplates.CreateError("Invalid date"), true);
                return;
            }

            var member = await memberManager.GetMember(user, ctx.Guild!);
            member.Birthday = date;
            await memberManager.UpdateMemberAsync(member);

            await ctx.RespondAsync(
                MessageTemplates.CreateSuccess($"Birthday updated to {date.ToShortDateString()} for {user.Mention}!"), true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing birthday change command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }

    [Command("add"), RequireGuild]
    public async Task AddBirthday(SlashCommandContext ctx, long year, long month, long day)
    {
        try
        {
            if (!DateTime.TryParse($"{year}-{month}-{day}", out var date))
            {
                await ctx.RespondAsync(MessageTemplates.CreateError("Invalid date"), true);
                return;
            }

            var member = await memberManager.GetMember(ctx.User, ctx.Guild!);
            member.Birthday = date;
            await memberManager.UpdateMemberAsync(member);

            await ctx.RespondAsync(
                MessageTemplates.CreateSuccess($"Birthday updated to {date.ToShortDateString()}"), true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing birthday add command for user {User}", ctx.User.Username);
            await ctx.RespondAsync(MessageTemplates.CreateError("An error occurred"), true);
        }
    }
    
    private async Task<List<ServerMember>> GetServerMembers(DiscordGuild guild) => await memberManager.GetMembersAsync(guild);
}
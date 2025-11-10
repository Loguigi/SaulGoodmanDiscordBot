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
    : BaseCommand<BirthdayCommands>(logger)
{
    [Command("check"), RequireGuild]
    public async Task CheckBirthday(SlashCommandContext ctx, DiscordUser user)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (await Validation.IsBot(ctx)) return;

            var member = await memberManager.GetMember(user, ctx.Guild!);

            if (await Validation.BirthdayNotSet(ctx, member)) return;

            await ctx.RespondAsync(MessageTemplates.CreateBirthdayCard(member), true);
            
        }, "birthday check");
    }

    [Command("next"), RequireGuild]
    public async Task NextBirthday(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await GetServerMembers(ctx.Guild!);
            var nextBirthday = memberManager.GetNextBirthday(members);

            await ctx.RespondAsync(MessageTemplates.CreateNextBirthday(nextBirthday, ctx.Guild!));
        }, "birthday next");
    }

    [Command("list"), RequireGuild]
    public async Task BirthdayList(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await GetServerMembers(ctx.Guild!);
            var filteredMembers = members.Where(x => x.Birthday.HasValue).ToList();

            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(
                    MessageTemplates.CreateBirthdayList(ctx.Guild!, filteredMembers, "1")));
        }, "birthday list");
    }

    [Command("change"), RequireGuild]
    [RequirePermissions(DiscordPermissions.Administrator)]
    public async Task ChangeBirthday(SlashCommandContext ctx, DiscordUser user, long year, long month, long day)
    {
        await ExecuteAsync(ctx, async () =>
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
                MessageTemplates.CreateSuccess($"Birthday updated to {date.ToShortDateString()} for {user.Mention}!"),
                true);
        }, "birthday change");
    }

    [Command("add"), RequireGuild]
    public async Task AddBirthday(SlashCommandContext ctx, long year, long month, long day)
    {
        await ExecuteAsync(ctx, async () =>
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
        }, "birthday add");
    }
    
    private async Task<List<ServerMember>> GetServerMembers(DiscordGuild guild) => await memberManager.GetMembersAsync(guild);
}
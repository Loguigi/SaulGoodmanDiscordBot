using System.ComponentModel;
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
[Command("birthday"), Description("Commands to manage user birthdays in the server")]
public class BirthdayCommands(
    ServerMemberManager memberManager,
    ILogger<BirthdayCommands> logger)
    : BaseCommand<BirthdayCommands>(logger)
{
    [Command("check"), Description("Check a member's birthday"), RequireGuild]
    public async Task CheckBirthday(SlashCommandContext ctx, DiscordUser user)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (await Validation.IsBot(ctx)) return;

            var member = await memberManager.GetMember(user, ctx.Guild!);

            if (await Validation.BirthdayNotSet(ctx, member)) return;

            await SendMessage(ctx, MessageTemplates.CreateBirthdayCard(member), true);

        }, "birthday check");
    }

    [Command("next"), Description("Check who's birthday is next"), RequireGuild]
    public async Task NextBirthday(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await GetServerMembers(ctx.Guild!);
            var nextBirthday = memberManager.GetNextBirthday(members);

            await SendMessage(ctx, MessageTemplates.CreateNextBirthday(nextBirthday, ctx.Guild!));
        }, "birthday next");
    }

    [Command("list"), Description("Gets a list of all members' birthdays in the server"), RequireGuild]
    public async Task BirthdayList(SlashCommandContext ctx)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var members = await GetServerMembers(ctx.Guild!);
            var filteredMembers = members.Where(x => x.Birthday.HasValue).ToList();

            await SendMessage(ctx, MessageTemplates.CreateBirthdayList(ctx.Guild!, filteredMembers, "1"));
        }, "birthday list");
    }

    [Command("change"), Description("Change a member's birthday"), RequireGuild]
    public async Task ChangeBirthday(SlashCommandContext ctx, DiscordUser user, long year, long month, long day)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (await Validation.IsBot(ctx)) return;

            if (!DateTime.TryParse($"{year}-{month}-{day}", out var date))
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Invalid date"), true);
                return;
            }

            var member = await memberManager.GetMember(user, ctx.Guild!);
            member.Birthday = date;
            await memberManager.UpdateMemberAsync(member);

            await SendMessage(ctx,
                MessageTemplates.CreateSuccess($"Birthday updated to {date.ToShortDateString()} for {user.Mention}!"),
                true);
        }, "birthday change");
    }

    [Command("add"), Description("Add your birthday"), RequireGuild]
    public async Task AddBirthday(SlashCommandContext ctx, long year, long month, long day)
    {
        await ExecuteAsync(ctx, async () =>
        {
            if (!DateTime.TryParse($"{year}-{month}-{day}", out var date))
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Invalid date"), true);
                return;
            }

            var member = await memberManager.GetMember(ctx.User, ctx.Guild!);
            member.Birthday = date;
            await memberManager.UpdateMemberAsync(member);

            await SendMessage(ctx, MessageTemplates.CreateSuccess($"Birthday updated to {date.ToShortDateString()}"),
                true);
        }, "birthday add");
    }
    
    private async Task<List<ServerMember>> GetServerMembers(DiscordGuild guild) => await memberManager.GetMembersAsync(guild);
}
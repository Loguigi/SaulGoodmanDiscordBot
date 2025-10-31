using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;

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
    ServerMemberManager memberManager)
{
    [Command("check"), RequireGuild]
    public async Task CheckBirthday(SlashCommandContext ctx, DiscordUser user)
    {
        if (await Validation.IsBot(ctx)) return;
        
        var member = await memberManager.GetMember(user, ctx.Guild!);

        if (await Validation.BirthdayNotSet(ctx, member)) return;
        
        await ctx.RespondAsync(MessageTemplates.CreateBirthdayCard(member), true);
    }

    [Command("next"), RequireGuild]
    public async Task NextBirthday(SlashCommandContext ctx)
    {
        var members = await GetServerMembers(ctx.Guild!);
        var nextBirthday = memberManager.GetNextBirthday(members);
        
        await ctx.RespondAsync(MessageTemplates.CreateNextBirthday(nextBirthday, ctx.Guild!));
    }

    [Command("list"), RequireGuild]
    public async Task BirthdayList(SlashCommandContext ctx)
    {
        var members = await GetServerMembers(ctx.Guild!);
        var filteredMembers = members.Where(x => x.Birthday.HasValue).ToList();
        
        await ctx.RespondWithModalAsync(
            new DiscordInteractionResponseBuilder(
                MessageTemplates.CreateBirthdayList(ctx.Guild!, filteredMembers, "1")));
    }

    [Command("change"), RequireGuild]
    [RequirePermissions(DiscordPermissions.Administrator)]
    public async Task ChangeBirthday(SlashCommandContext ctx, DiscordUser user, long year, long month, long day)
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

    [Command("add"), RequireGuild]
    public async Task AddBirthday(SlashCommandContext ctx, long year, long month, long day)
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
    
    private async Task<List<ServerMember>> GetServerMembers(DiscordGuild guild) => await memberManager.GetMembersAsync(guild);
}
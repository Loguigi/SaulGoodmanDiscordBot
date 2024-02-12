/*
    BirthdayCommands.cs

    Provides commands for managing user birthdays

    *Command List*
    -check (user): checks birthday of specified user
    -add (user, date): sets user's birthday to specified date
    -list: displays list of birthdays in server
    -next: shows the user with the next birthday
*/

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using SaulGoodmanBot.Handlers;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Controllers;
using SaulGoodmanBot.Helpers;
using System.Reflection;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("birthday", "Commands for friend birthdays")]
public class BirthdayCommands : ApplicationCommandModule {
    [SlashCommand("check", "Check the birthday of a user")]
    public async Task CheckBirthday(InteractionContext ctx,
        [Option("user", "Birthday to check")] DiscordUser user) {

        if (user.IsBot) {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        try {
            var birthday = new ServerBirthdays(ctx.Client, ctx.Guild)[user] ?? 
                throw new Exception($"{user.Mention} has not input their birthday");

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{birthday.User.GlobalName}'s birthday", "", birthday.User.AvatarUrl)
                .WithTitle(birthday.ToString())
                .WithColor(DiscordColor.Lilac);

            await ctx.CreateResponseAsync(embed, ephemeral:true);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Birthday")]
    public async Task ContextCheckBirthday(ContextMenuContext ctx) {
        if (ctx.TargetUser.IsBot) {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        try {
            var birthday = new ServerBirthdays(ctx.Client, ctx.Guild)[ctx.TargetUser] ?? 
                throw new Exception($"{ctx.TargetUser.Mention} has not input their birthday");

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{birthday.User.GlobalName}'s birthday", "", birthday.User.AvatarUrl)
                .WithTitle(birthday.ToString())
                .WithColor(DiscordColor.Lilac);

            await ctx.CreateResponseAsync(embed, ephemeral:true);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("add", "Add your birthday")]
    public async Task AddBirthday(InteractionContext ctx,
        [ChoiceProvider(typeof(MonthChoiceProvider))][Option("month", "Month of birthday")] long month,
        [Option("day", "Day of birthday")][Minimum(1)][Maximum(31)] long day,
        [Option("year", "Year of birthday")][Minimum(2024-100)][Maximum(2024)] long year) {
        
        try {
            var birthdays = new ServerBirthdays(ctx.Client, ctx.Guild);
            if (!DateTime.TryParse($"{year}, {month} {day}", out DateTime date))
                throw new Exception("Invalid date");

            await ctx.CreateResponseAsync(StandardOutput.Success($"Your birthday is changed to {month}/{day}/{year}"));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("change", "Change a user's birthday")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task ChangeBirthday(InteractionContext ctx,
        [Option("user", "User's birthday to add")] DiscordUser user, 
        [ChoiceProvider(typeof(MonthChoiceProvider))][Option("month", "Month of birthday")] long month,
        [Option("day", "Day of birthday")][Minimum(1)][Maximum(31)] long day,
        [Option("year", "Year of birthday")][Minimum(2023-100)][Maximum(2023)] long year) {

        if (user.IsBot) {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }
        
        try {
            var birthdays = new ServerBirthdays(ctx.Client, ctx.Guild);
            if (!DateTime.TryParse($"{year}, {month} {day}", out DateTime date))
                throw new Exception("Invalid date");

            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.Mention}'s birthday changed to {month}/{day}/{year}"));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("list", "Lists all the birthdays of your friends")]
    public async Task ListBirthdays(InteractionContext ctx) {
        try {
            var birthdays = new ServerBirthdays(ctx.Client, ctx.Guild);
            var interactivity = new InteractivityHelper<Birthday>(ctx.Client, birthdays.Birthdays, IDHelper.Birthdays.LIST, "1", 10, "There are no birthdays");
            
            // print all birthdays
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Birthdays")
                .WithDescription(interactivity.IsEmpty())
                .WithColor(DiscordColor.Magenta)
                .WithFooter(interactivity.PageStatus);

            foreach (var birthday in interactivity) {
                embed.Description += $"### {birthday.User.Mention}: {birthday.BDay:MMMM d} `({birthday.Age + 1})`\n";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

            ctx.Client.ComponentInteractionCreated -= BirthdayHandler.HandleBirthdayList;
            ctx.Client.ComponentInteractionCreated += BirthdayHandler.HandleBirthdayList;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    
    [SlashCommand("next", "Finds the next upcoming birthday")]
    public async Task NextBirthday(InteractionContext ctx) {
        var birthdays = new ServerBirthdays(ctx.Client, ctx.Guild);

        var nextBirthday = birthdays.Next;
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Next Birthday")
            .WithThumbnail(nextBirthday.User.AvatarUrl)
            .WithDescription($"### {nextBirthday.User.Mention}\n{nextBirthday.BDay:MMMM d, yyyy}")
            .WithColor(DiscordColor.SpringGreen);

        await ctx.CreateResponseAsync(embed);
    }
}

public class MonthChoiceProvider : IChoiceProvider
{
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
        await Task.CompletedTask;
        return new DiscordApplicationCommandOptionChoice[]
        {
            new("January", 1),
            new("February", 2),
            new("March", 3),
            new("April", 4),
            new("May", 5),
            new("June", 6),
            new("July", 7),
            new("August", 8),
            new("September", 9),
            new("October", 10),
            new("November", 11),
            new("December", 12),
        };
    }
}

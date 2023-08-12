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
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("birthday", "Commands for friend birthdays")]
public class BirthdayCommands : ApplicationCommandModule {
    [SlashCommand("check", "Check the birthday of a user")]
    public async Task CheckBirthday(InteractionContext ctx,
        [Option("user", "Birthday to check")] DiscordUser user) {

        var bdayList = new Birthdays(ctx.Guild);
        var bday = bdayList.Find(user);

        if (bday == bdayList.DATE_ERROR) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"No birthday found for {user.GlobalName}"), ephemeral:true);
        } else {
            var response = new DiscordEmbedBuilder()
                .WithAuthor($"{user.GlobalName}'s birthday", "", user.AvatarUrl)
                .WithTitle(bday.ToString("MMMM d, yyyy"))
                .WithColor(DiscordColor.Lilac);

            await ctx.CreateResponseAsync(response, ephemeral:true);
        }
    }

    [SlashCommandPermissions(Permissions.Administrator)]
    [SlashCommand("add", "Add a user's birthday")]
    public async Task AddBirthday(InteractionContext ctx,
        [Option("user", "User's birthday to add")] DiscordUser user, 
        [ChoiceProvider(typeof(MonthChoiceProvider))][Option("month", "Month of birthday")] long month,
        [Option("day", "Day of birthday")][Minimum(1)][Maximum(31)] long day,
        [Option("year", "Year of birthday")][Minimum(2023-100)][Maximum(2023)] long year) {
        
        var bday = new Birthdays(ctx.Guild);
        var date = new DateTime((int)year, (int)month, (int)day);

        if (bday.Find(user) == bday.DATE_ERROR) {
            // birthday not saved yet, so save birthday
            bday.Add(new Birthday(user, date));

            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.Mention}'s birthday set to {month}/{day}/{year}"));
        } else {
            // birthday already exists, so update birthday
            bday.Update(new Birthday(user, date));

            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.Mention}'s birthday changed to {month}/{day}/{year}"));
        }
    }

    [SlashCommand("list", "Lists all the birthdays of your friends")]
    public async Task ListBirthdays(InteractionContext ctx) {
        var bdayList = new Birthdays(ctx.Guild);

        if (bdayList.IsEmpty()) {
            // error: no birthdays in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no birthdays in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // print all birthdays
            var response = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Birthdays")
                .WithDescription("");

            foreach (var birthday in bdayList.BirthdayList) {
                response.Description += $"{birthday.User.Mention}: {birthday.BDay:MMMM d} `({birthday.GetAge() + 1})`\n";
            }

            await ctx.CreateResponseAsync(response);
        }
    }
    
    [SlashCommand("next", "Finds the next upcoming birthday")]
    public async Task NextBirthday(InteractionContext ctx) {
        var bdayList = new Birthdays(ctx.Guild);

        if (bdayList.IsEmpty()) {
            // error: no birthdays in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no birthdays in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            var nextBirthday = bdayList.Next();
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Next Birthday")
                .WithThumbnail(nextBirthday.User.AvatarUrl)
                .WithDescription($"### {nextBirthday.User.Mention}\n{nextBirthday.BDay:MMMM d, yyyy}")
                .WithColor(DiscordColor.SpringGreen);

            await ctx.CreateResponseAsync(response);
        }
    }
}

public class MonthChoiceProvider : IChoiceProvider
{
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
        await Task.CompletedTask;
        return new DiscordApplicationCommandOptionChoice[]
        {
            new DiscordApplicationCommandOptionChoice("January", 1),
            new DiscordApplicationCommandOptionChoice("February", 2),
            new DiscordApplicationCommandOptionChoice("March", 3),
            new DiscordApplicationCommandOptionChoice("April", 4),
            new DiscordApplicationCommandOptionChoice("May", 5),
            new DiscordApplicationCommandOptionChoice("June", 6),
            new DiscordApplicationCommandOptionChoice("July", 7),
            new DiscordApplicationCommandOptionChoice("August", 8),
            new DiscordApplicationCommandOptionChoice("September", 9),
            new DiscordApplicationCommandOptionChoice("October", 10),
            new DiscordApplicationCommandOptionChoice("November", 11),
            new DiscordApplicationCommandOptionChoice("December", 12),
        };
    }
}

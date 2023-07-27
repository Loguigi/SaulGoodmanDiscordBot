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

        var bdayList = new Birthdays(ctx.Guild.Id, ctx);
        var bday = bdayList.Find(user);

        if (bday == bdayList.DATE_ERROR) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"No birthday found for {user.GlobalName}"), ephemeral:true);
        } else {
            var response = new DiscordEmbedBuilder()
                .WithAuthor($"{user.GlobalName}'s birthday", "", user.AvatarUrl)
                .WithTitle(bday.ToString("MMMM d, yyyy"))
                .WithThumbnail(ImageHelper.Images["50"])
                .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Lilac);

            await ctx.CreateResponseAsync(response);
        }
    }

    [SlashCommandPermissions(Permissions.Administrator)]
    [SlashCommand("add", "Add a user's birthday")]
    public async Task AddBirthday(InteractionContext ctx,
        [Option("user", "User's birthday to add")] DiscordUser user, 
        [ChoiceProvider(typeof(MonthChoiceProvider))][Option("month", "Month of birthday")] long month,
        [Option("day", "Day of birthday")][Minimum(1)][Maximum(31)] long day,
        [Option("year", "Year of birthday")][Minimum(2023-100)][Maximum(2023)] long year) {
        
        var bday = new Birthdays(ctx.Guild.Id, ctx);
        var date = new DateTime((int)year, (int)month, (int)day);

        if (bday.Find(user) == bday.DATE_ERROR) {
            // birthday not saved yet, so save birthday
            bday.Add(new Birthday(user, date));

            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.GlobalName}'s birthday set to {month}/{day}/{year}"));
        } else {
            // birthday already exists, so update birthday
            bday.Update(new Birthday(user, date));

            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.GlobalName}'s birthday changed to {month}/{day}/{year}"));
        }
    }

    [SlashCommand("list", "Lists all the birthdays of your friends")]
    public async Task ListBirthdays(InteractionContext ctx) {
        var bdayList = new Birthdays(ctx.Guild.Id, ctx);

        if (bdayList.GetBirthdays().Count == 0) {
            // error: no birthdays in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no birthdays in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // print all birthdays
            var response = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Birthdays")
                .WithDescription("")
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Blurple);

            foreach (var birthday in bdayList.GetBirthdays()) {
                response.Description += $"{birthday.User.GlobalName}: {birthday.BDay.ToString("MMMM d")} `({birthday.GetAge() + 1})`\n";
            }

            await ctx.CreateResponseAsync(response);
        }
    }
    
    [SlashCommand("next", "Finds the next upcoming birthday")]
    public async Task NextBirthday(InteractionContext ctx) {
        var bdayList = new Birthdays(ctx.Guild.Id, ctx);

        if (bdayList.GetBirthdays().Count == 0) {
            // error: no birthdays in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no birthdays in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            var nextBirthday = bdayList.Next();
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Next Birthday")
                .WithTitle(nextBirthday.User.GlobalName)
                .WithThumbnail(nextBirthday.User.AvatarUrl)
                .WithDescription(nextBirthday.BDay.ToString("MMMM d, yyyy"))
                .WithFooter(ctx.Guild.Name, ctx.Guild.IconUrl)
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.SpringGreen);

            await ctx.CreateResponseAsync(response);
        }
    }
}

public class MonthChoiceProvider : IChoiceProvider
{
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
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

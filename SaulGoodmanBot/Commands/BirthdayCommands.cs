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
using SaulGoodmanBot.Library.Birthdays;
using SaulGoodmanBot.Library.Helpers;

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

        var birthday = new ServerBirthdays(ctx.Guild).Find(user);

        if (birthday.HasNoBirthday()) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"No birthday found for {user.GlobalName}"), ephemeral:true);
            return;
        }

        var embed = new DiscordEmbedBuilder()
            .WithAuthor($"{user.GlobalName}'s birthday", "", user.AvatarUrl)
            .WithTitle(birthday.ToString())
            .WithColor(DiscordColor.Lilac);

        await ctx.CreateResponseAsync(embed, ephemeral:true);
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Birthday")]
    public async Task ContextCheckBirthday(ContextMenuContext ctx) {
        if (ctx.TargetUser.IsBot) {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        var birthday = new ServerBirthdays(ctx.Guild).Find(ctx.TargetUser);

        if (birthday.HasNoBirthday()) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"No birthday found for {ctx.TargetUser.GlobalName}"), ephemeral:true);
            return;
        }

        var embed = new DiscordEmbedBuilder()
            .WithAuthor($"{ctx.TargetUser.GlobalName}'s birthday", "", ctx.TargetUser.AvatarUrl)
            .WithTitle(birthday.ToString())
            .WithColor(DiscordColor.Lilac);

        await ctx.CreateResponseAsync(embed, ephemeral:true);
    }

    [SlashCommand("add", "Add your birthday")]
    public async Task AddBirthday(InteractionContext ctx,
        [ChoiceProvider(typeof(MonthChoiceProvider))][Option("month", "Month of birthday")] long month,
        [Option("day", "Day of birthday")][Minimum(1)][Maximum(31)] long day,
        [Option("year", "Year of birthday")][Minimum(2023-100)][Maximum(2023)] long year) {
        
        var birthdays = new ServerBirthdays(ctx.Guild);
        var date = new DateTime((int)year, (int)month, (int)day);

        if (birthdays.Find(ctx.User).HasNoBirthday()) {
            birthdays.Edit(DataOperations.Add, new Birthday(ctx.User, date));
            await ctx.CreateResponseAsync(StandardOutput.Success($"Your birthday is set to {month}/{day}/{year}"), ephemeral:true);
            return;
        }

        // birthday already exists, so update birthday
        birthdays.Edit(DataOperations.Update, new Birthday(ctx.User, date));

        await ctx.CreateResponseAsync(StandardOutput.Success($"Your birthday is changed to {month}/{day}/{year}"));
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
        
        var birthdays = new ServerBirthdays(ctx.Guild);
        var date = new DateTime((int)year, (int)month, (int)day);

        if (birthdays.Find(user).HasNoBirthday()) {
            birthdays.Edit(DataOperations.Add, new Birthday(user, date));
            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.Mention}'s birthday set to {month}/{day}/{year}"));
            return;
        }

        // birthday already exists, so update birthday
        birthdays.Edit(DataOperations.Update, new Birthday(user, date));

        await ctx.CreateResponseAsync(StandardOutput.Success($"{user.Mention}'s birthday changed to {month}/{day}/{year}"));
    }

    [SlashCommand("list", "Lists all the birthdays of your friends")]
    public async Task ListBirthdays(InteractionContext ctx) {
        var birthdays = new ServerBirthdays(ctx.Guild);

        if (birthdays.IsEmpty()) {
            // error: no birthdays in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no birthdays in {ctx.Guild.Name}"), ephemeral:true);
            return;
        }

        var interactivity = new InteractivityHelper<Birthday>(ctx.Client, birthdays.GetBirthdays(), IDHelper.Birthdays.LIST, "1", "There are no birthdays");
        
        // print all birthdays
        var embed = new DiscordEmbedBuilder()
            .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
            .WithTitle("Birthdays")
            .WithDescription(interactivity.IsEmpty())
            .WithColor(DiscordColor.Magenta)
            .WithFooter(interactivity.PageStatus());

        foreach (var birthday in interactivity.GetPage()) {
            embed.Description += $"### {birthday.User.Mention}: {birthday.BDay:MMMM d} `({birthday.GetAge() + 1})`\n";
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

        ctx.Client.ComponentInteractionCreated -= BirthdayHandler.HandleBirthdayList;
        ctx.Client.ComponentInteractionCreated += BirthdayHandler.HandleBirthdayList;
    }
    
    [SlashCommand("next", "Finds the next upcoming birthday")]
    public async Task NextBirthday(InteractionContext ctx) {
        var birthdays = new ServerBirthdays(ctx.Guild);

        if (birthdays.IsEmpty()) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no birthdays in {ctx.Guild.Name}"), ephemeral:true);
            return;
        }

        var nextBirthday = birthdays.Next();
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

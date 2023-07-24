using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("birthday", "Commands for friend birthdays")]
public class BirthdayCommands : ApplicationCommandModule {
    [SlashCommand("check", "Check the birthday of a user")]
    public async Task CheckBirthday(InteractionContext cmd,
        [Option("user", "Birthday to check")] DiscordUser user) {

        var bday = new Birthdays(cmd);
        
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("add", "Add a user's birthday")]
    public async Task AddBirthday(InteractionContext cmd,
        [Option("user", "User's birthday to add")] DiscordUser user, 
        [ChoiceProvider(typeof(MonthChoiceProvider))][Option("month", "Month of birthday")] int month,
        [Option("day", "Day of birthday")][Minimum(1)][Maximum(31)] int day,
        [Option("year", "Year of birthday")][Minimum(2023-100)][Maximum(2023)] int year) {
        
        var bday = new Birthdays(cmd);
        var date = new DateOnly(year, month, day);
        bday.AddBirthday(new Birthday(cmd.User, date));

        // TODO: output
        await cmd.CreateResponseAsync("not implemented");
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

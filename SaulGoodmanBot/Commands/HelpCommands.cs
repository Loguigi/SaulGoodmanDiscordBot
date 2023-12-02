using DSharpPlus.SlashCommands;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("help", "Help commands for all bot features")]
public class HelpCommands : ApplicationCommandModule {
    [SlashCommand("setup", "Help for setting up the bot for the server")]
    public async Task SetupHelp(InteractionContext ctx) {

    }

    [SlashCommand("wheel", "Help for the /wheel commands")]
    public async Task WheelHelp(InteractionContext ctx) {

    }

    [SlashCommand("birthday", "Help for the /birthday commands")]
    public async Task BirthdayHelp(InteractionContext ctx) {

    }

    [SlashCommand("roles", "Help for the /role commands")]
    public async Task RoleHelp(InteractionContext ctx) {

    }

    [SlashCommand("schedule", "Help for the /schedule commands")]
    public async Task ScheduleHelp(InteractionContext ctx) {

    }

    [SlashCommand("levels", "Help for the levelling system")]
    public async Task LevelHelp(InteractionContext ctx) {
        
    }

    [SlashCommand("minecraft", "Help for the /mc commands")]
    public async Task MinecraftHelp(InteractionContext ctx) {

    }

    [SlashCommand("secret_santa", "Help for the /santa commands")]
    public async Task SantaHelp(InteractionContext ctx) {

    }

    [SlashCommand("misc", "List of misc and fun commands to use")]
    public async Task MiscHelp(InteractionContext ctx) {

    }
}
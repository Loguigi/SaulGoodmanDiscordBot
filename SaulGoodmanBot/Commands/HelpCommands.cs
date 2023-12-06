using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Handlers;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("help", "Help commands for all bot features")]
public class HelpCommands : ApplicationCommandModule {
    [SlashCommand("setup", "Help for setting up the bot for the server")]
    public async Task SetupHelp(InteractionContext ctx) {
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Saul Goodman", "", ImageHelper.Images["Heisenberg"])
            .WithTitle(HelpText.Setup.First().Key)
            .WithDescription(HelpText.Setup.First().Value)
            .WithThumbnail(ImageHelper.Images["Saul"])
            .WithColor(DiscordColor.Orange);
        
        var pages = new List<DiscordSelectComponentOption>();
        foreach (var p in HelpText.Setup.Keys) {
            pages.Add(new DiscordSelectComponentOption(p, p));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.Help.SETUP, "Select a page...", pages);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

        ctx.Client.ComponentInteractionCreated -= HelpHandler.HandleSetupHelp;
        ctx.Client.ComponentInteractionCreated += HelpHandler.HandleSetupHelp;
    }

    [SlashCommand("wheel", "Help for the /wheel commands")]
    public async Task WheelHelp(InteractionContext ctx) {
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Wheel Picker", "", ImageHelper.Images["PS2Jesse"])
            .WithTitle(HelpText.WheelPicker.First().Key)
            .WithDescription(HelpText.WheelPicker.First().Value)
            .WithColor(DiscordColor.Gold);
        
        var pages = new List<DiscordSelectComponentOption>();
        foreach (var p in HelpText.WheelPicker.Keys) {
            pages.Add(new DiscordSelectComponentOption(p, p));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.Help.WHEELPICKER, "Select a page...", pages);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

        ctx.Client.ComponentInteractionCreated -= HelpHandler.HandleWheelPickerHelp;
        ctx.Client.ComponentInteractionCreated += HelpHandler.HandleWheelPickerHelp;
    }

    [SlashCommand("birthday", "Help for the /birthday commands")]
    public async Task BirthdayHelp(InteractionContext ctx) {
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Birthdays", "", ImageHelper.Images["SmilingGus"])
            .WithTitle(HelpText.Birthday.First().Key)
            .WithDescription(HelpText.Birthday.First().Value)
            .WithColor(DiscordColor.HotPink);
        
        var pages = new List<DiscordSelectComponentOption>();
        foreach (var p in HelpText.Birthday.Keys) {
            pages.Add(new DiscordSelectComponentOption(p, p));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.Help.BIRTHDAY, "Select a page...", pages);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

        ctx.Client.ComponentInteractionCreated -= HelpHandler.HandleBirthdayHelp;
        ctx.Client.ComponentInteractionCreated += HelpHandler.HandleBirthdayHelp;
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
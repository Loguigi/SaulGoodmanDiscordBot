/*
    WheelPickerCommands.cs

    Commands to control a wheel picker

    *Slash Commands:
*/

using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Handlers;
using System.Reflection;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("wheel", "Commands to manage wheel pickers")]
[GuildOnly]
public class WheelPickerCommands : ApplicationCommandModule 
{
    [SlashCommand("create", "Creates a new wheel picker")]
    public async Task CreateWheel(InteractionContext ctx,
        [Option("name", "Name of the wheel picker")][MaximumLength(100)] string name,
        [Option("first_option", "First option to add to the wheel")][MaximumLength(100)] string option,
        [Option("image", "Image for the wheel")] DiscordAttachment? img = null )
    {
        WheelPickers wheels = new(ctx.Guild);
        await wheels.CreateWheel(new WheelPickers.Wheel(name, img?.Url ?? string.Empty), option);
        
        await ctx.CreateResponseAsync(StandardOutput.Success($"`{name}` wheel added"), ephemeral:true);
    }

    [SlashCommand("add", "Adds new options to the wheel")]
    public async Task AddWheelOption(InteractionContext ctx) 
    {
        var wheels = new WheelPickers(ctx.Guild);
        
        if (wheels.Wheels.Count == 0)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error("No wheels found in server"), ephemeral: true);
            return;
        }
            
        var options = new List<DiscordSelectComponentOption>();
        foreach (var wheel in wheels.Wheels) 
        {
            options.Add(new DiscordSelectComponentOption(wheel.Name, wheel.Name, $"{wheel.Options.Count} options"));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.WheelPicker.Add, "Select a wheel", options, false);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Add to wheel");

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));
            
        ctx.Client.ComponentInteractionCreated -= WheelPickerHandler.HandleAdd;
        ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleAdd;
    }

    [SlashCommand("spin", "Spins the chosen wheel for a value")]
    public async Task SpinWheel(InteractionContext ctx) 
    {
        var wheels = new WheelPickers(ctx.Guild);
        
        if (wheels.Wheels.Count == 0)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error("No wheels found in server"), ephemeral: true);
            return;
        }

        var options = new List<DiscordSelectComponentOption>();
        foreach (var wheel in wheels.Wheels) 
        {
            if (wheel.Options.Count != 0)
                options.Add(new DiscordSelectComponentOption(wheel.Name, wheel.Name, $"{wheel.AvailableOptions.Count} options, {wheel.RemovedOptions.Count} removed"));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.WheelPicker.Spin, "Select a wheel", options, false);

        // display prompt
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Spinning...")
            .WithColor(DiscordColor.Cyan);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

        ctx.Client.ComponentInteractionCreated -= WheelPickerHandler.HandleSpin;
        ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleSpin;
    }

    [SlashCommand("delete", "Deletes a wheel picker or option from a wheel picker")]
    public async Task DeleteWheel(InteractionContext ctx) 
    {
        var wheels = new WheelPickers(ctx.Guild);
        
        if (wheels.Wheels.Count == 0)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error("No wheels found in server"), ephemeral: true);
            return;
        }

        var options = new List<DiscordSelectComponentOption>();
        foreach (var wheel in wheels.Wheels) 
        {
            if (wheel.Options.Count >= 1)
                options.Add(new DiscordSelectComponentOption(wheel.Name, wheel.Name, $"{wheel.Options.Count} options"));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.WheelPicker.DeleteWheel, "Select a wheel", options, false);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Delete Wheel")
            .WithColor(DiscordColor.DarkRed);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

        ctx.Client.ComponentInteractionCreated -= WheelPickerHandler.HandleDeleteWheelSelection;
        ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleDeleteWheelSelection;
    }

    [SlashCommand("list", "Shows list of options in a wheel")]
    public async Task ListWheelOptions(InteractionContext ctx) 
    {
        var wheels = new WheelPickers(ctx.Guild);
        
        if (wheels.Wheels.Count == 0)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error("No wheels found in server"), ephemeral: true);
            return;
        }
        
        // add server wheels to dropdown
        var wheelOptions = new List<DiscordSelectComponentOption>();
        foreach (var wheel in wheels.Wheels) 
        {
            wheelOptions.Add(new DiscordSelectComponentOption(wheel.Name, wheel.Name, $"{wheel.AvailableOptions.Count} options"));
        }
        var wheelDropdown = new DiscordSelectComponent(IDHelper.WheelPicker.List, "Select a wheel", wheelOptions, false);

        // display prompt
        var prompt = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("List Wheel Options")
                .WithColor(DiscordColor.MidnightBlue))
            .AddComponents(wheelDropdown);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

        ctx.Client.ComponentInteractionCreated -= WheelPickerHandler.HandleList;
        ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleList;
    }

    [SlashCommand("reload", "Reloads a saved wheel and restores deleted options")]
    public async Task ReloadWheel(InteractionContext ctx) 
    {
        var wheels = new WheelPickers(ctx.Guild);

        if (wheels.Wheels.Count == 0)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error("No wheels found in server"), ephemeral: true);
            return;
        }

        var options = new List<DiscordSelectComponentOption>();
        foreach (var wheel in wheels.Wheels) 
        {
            options.Add(new DiscordSelectComponentOption(wheel.Name, wheel.Name, $"{wheel.Options.Count} options"));  
        }
        var dropdown = new DiscordSelectComponent(IDHelper.WheelPicker.ReloadWheel, "Select a wheel", options, false);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Reload Wheel")
            .WithColor(DiscordColor.Teal);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

        ctx.Client.ComponentInteractionCreated -= WheelPickerHandler.HandleReload;
        ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleReload;
    }
}

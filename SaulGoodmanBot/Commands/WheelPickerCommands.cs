/*
    WheelPickerCommands.cs

    Commands to control a wheel picker

    *Slash Commands:
*/

using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Handlers;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("wheel", "Commands to manage wheel pickers")]
[GuildOnly]
public class WheelPickerCommands : ApplicationCommandModule {
    [SlashCommand("create", "Creates a new wheel picker")]
    [GuildOnly]
    public async Task CreateWheel(InteractionContext ctx,
        [Option("name", "Name of the wheel picker")][MaximumLength(100)] string name,
        [Option("value1", "First value to add to the wheel")][MaximumLength(100)] string value1,
        [Option("value2", "Second value to add to the wheel")][MaximumLength(100)] string value2,
        [Option("image", "Image for the wheel")] DiscordAttachment? img = null ) {
        
        var serverWheels = new WheelPickers(ctx.Guild);
        
        if (serverWheels.Contains(name)) {
            // error: wheel already exists
            await ctx.CreateResponseAsync(StandardOutput.Error($"`{name}` already exists in {ctx.Guild.Name}"), ephemeral:true);
        } else if (serverWheels.IsFull()) {
            // error: no more wheels can be added
            await ctx.CreateResponseAsync(StandardOutput.Error($"Too many wheels in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // saves new wheel
            if (img == null) {
                serverWheels.Add(new Wheel(name, new List<string>(){value1, value2}, null));
            } else {
                serverWheels.Add(new Wheel(name, new List<string>(){value1, value2}, img.Url));
            }

            await ctx.CreateResponseAsync(StandardOutput.Success($"`{name}` wheel added"), ephemeral:true);
        }
    }

    [SlashCommand("add", "Adds new options to the wheel")]
    public async Task AddWheelOption(InteractionContext ctx) {
        var serverWheels = new WheelPickers(ctx.Guild);

        if (serverWheels.Wheels.Count == 0) {
            // error: no wheels in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no wheels to add to in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("adddropdown", "Select a wheel", wheelOptions, false);

            // display prompt
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Add Wheel Options"))
                .AddComponents(wheelDropdown);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));
            
            ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleAdd;
        }
    }

    [SlashCommand("spin", "Spins the chosen wheel for a value")]
    public async Task SpinWheel(InteractionContext ctx) {
        var serverWheels = new WheelPickers(ctx.Guild);

        if (serverWheels.Wheels.Count == 0) {
            // error: no wheels in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no wheels to spin in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("spindropdown", "Select a wheel", wheelOptions, false);

            // display prompt
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Spinning...")
                    .WithColor(DiscordColor.Gold))
                .AddComponents(wheelDropdown);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            ctx.Client.ComponentInteractionCreated -= WheelPickerHandler.HandleSpin;
            ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleSpin;
        }
    }

    [SlashCommand("delete", "Deletes a wheel picker or option from a wheel picker")]
    public async Task DeleteWheel(InteractionContext ctx) {
        var serverWheels = new WheelPickers(ctx.Guild);

        if (serverWheels.Wheels.Count == 0) {
            // error: no wheels in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no wheels to delete in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("deletewheeldropdown", "Select a wheel", wheelOptions, false);

            // display prompt
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Delete Wheel")
                    .WithColor(DiscordColor.DarkRed))
                .AddComponents(wheelDropdown);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            ctx.Client.ComponentInteractionCreated += WheelPickerHandler.HandleDelete;
        }
    }

    [SlashCommand("list", "Shows list of options in a wheel")]
    public async Task ListWheelOptions(InteractionContext ctx) {
        var serverWheels = new WheelPickers(ctx.Guild);
        
        if (serverWheels.Wheels.Count == 0) {
            // error: no wheels in server
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no wheels to display in {ctx.Guild.Name}"), ephemeral:true);
        } else {
            // add server wheels to dropdown
            var wheelOptions = new List<DiscordSelectComponentOption>();
            foreach (var wheel in serverWheels.Wheels) {
                wheelOptions.Add(new DiscordSelectComponentOption(wheel.Key, wheel.Key, $"{wheel.Value.Options.Count} options"));
            }
            var wheelDropdown = new DiscordSelectComponent("listdropdown", "Select a wheel", wheelOptions, false);

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
    }
}

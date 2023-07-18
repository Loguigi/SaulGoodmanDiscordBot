/*
    WheelPickerCommands.cs

    Commands to control a wheel picker

    *Slash Commands:
*/

using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Commands;

public class WheelPickerCommands : ApplicationCommandModule {
    [SlashCommand("createwheel", "Creates a new wheel picker")]
    public async Task CreateWheel(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("addwheeloption", "Adds a new option to the wheel")]
    public async Task AddWheelOption(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("spinwheel", "Spins the chosen wheel for a value")]
    public async Task SpinWheel(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("deletewheeloption", "Deletes an existing option from a wheel")]
    public async Task DeleteWheelOption(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("deletewheel", "Deletes an existing wheel picker")]
    public async Task DeleteWheel(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("listwheeloptions", "Shows options in a wheel")]
    public async Task ListWheelOptions(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("listwheels", "Shows all available wheel pickers")]
    public async Task ListWheels(InteractionContext cmd) {
        // TODO
        await cmd.CreateResponseAsync("not implemented");
    }
}
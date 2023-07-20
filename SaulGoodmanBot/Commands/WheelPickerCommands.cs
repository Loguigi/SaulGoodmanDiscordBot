/*
    WheelPickerCommands.cs

    Commands to control a wheel picker

    *Slash Commands:
*/

using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

public class WheelPickerCommands : ApplicationCommandModule {
    [SlashCommand("createwheel", "Creates a new wheel picker")]
    public async Task CreateWheel(InteractionContext cmd,
                                  [Option("name", "Name of the wheel picker")] string name,
                                  [Option("value1", "First value to add to the wheel")] string value1,
                                  [Option("value2", "Second value to add to the wheel")] string value2) {
        // TODO: add validation to prevent duplicate wheels
        var wheel = new WheelPicker(cmd.Guild, name, new List<string>(){value1, value2});
        wheel.SaveWheel();
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("addwheeloption", "Adds a new option to the wheel")]
    public async Task AddWheelOption(InteractionContext cmd,
                                  [Option("name", "Name of the wheel picker")] string name,
                                  [Option("option", "Option to be added to the wheel")] string option) {
        // TODO: add validation if wheel exists
        var wheel = new WheelPicker(cmd.Guild, name, new List<string>(){option});
        await cmd.CreateResponseAsync("not implemented");
    }

    [SlashCommand("spinwheel", "Spins the chosen wheel for a value")]
    public async Task SpinWheel(InteractionContext cmd, 
                                [Option("name", "Name of the wheel picker")] string name) {
        var wheel = new WheelPicker(cmd.Guild, name);
        if (wheel.WheelOptions.Count == 0) {
            // wheel does not exist
            await cmd.CreateResponseAsync($"`Wheel '{name}' does not exist`");
        } else {
            var result = wheel.SpinWheel();
        }
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
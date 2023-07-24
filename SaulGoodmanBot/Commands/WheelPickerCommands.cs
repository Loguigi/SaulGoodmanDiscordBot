/*
    WheelPickerCommands.cs

    Commands to control a wheel picker

    *Slash Commands:
*/

using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("wheel", "Commands to manage wheel pickers")]
[GuildOnly]
public class WheelPickerCommands : ApplicationCommandModule {
    [SlashCommand("create", "Creates a new wheel picker")]
    [GuildOnly]
    public async Task CreateWheel(InteractionContext cmd,
        [Option("name", "Name of the wheel picker")][MaximumLength(100)] string name,
        [Option("value1", "First value to add to the wheel")][MaximumLength(100)] string value1,
        [Option("value2", "Second value to add to the wheel")][MaximumLength(100)] string value2,
        [Option("image", "Image for the wheel")] DiscordAttachment? img = null ) {
        
        var serverWheels = new WheelPickers(cmd);
        
        if (serverWheels.Contains(name)) {
            // error: wheel already exists
            await cmd.CreateResponseAsync(StandardOutput.Error($"`{name}` already exists in {cmd.Guild.Name}"), ephemeral:true);
        } else {
            // saves new wheel
            if (img == null) {
                serverWheels.Add(new Wheel(name, new List<string>(){value1, value2}, null));
            } else {
                serverWheels.Add(new Wheel(name, new List<string>(){value1, value2}, img.Url));
            }

            await cmd.CreateResponseAsync(StandardOutput.Success($"`{name}` wheel added"));
        }
    }

    [SlashCommand("add", "Adds a new option to the wheel")]
    public async Task AddWheelOption(InteractionContext cmd,
        [Option("name", "Name of the wheel picker")] string name,
        [Option("option", "Option to be added to the wheel")] string option,
        [Option("option2", "Option to be added to the wheel")] string option2="",
        [Option("option3", "Option to be added to the wheel")] string option3="",
        [Option("option4", "Option to be added to the wheel")] string option4="",
        [Option("option5", "Option to be added to the wheel")] string option5="",
        [Option("option6", "Option to be added to the wheel")] string option6="",
        [Option("option7", "Option to be added to the wheel")] string option7="",
        [Option("option8", "Option to be added to the wheel")] string option8="",
        [Option("option9", "Option to be added to the wheel")] string option9="",
        [Option("option10", "Option to be added to the wheel")] string option10="") {
        
        var serverWheels = new WheelPickers(cmd);
        var optionsInput = new List<string>() {option, option2, option3, option4, option5, option6, option7, option8, option9, option10};
        var options = optionsInput.Where(o => o != "").ToList();

        if (!serverWheels.Contains(name)) {
            // error: wheel doesn't exist
            await cmd.CreateResponseAsync(StandardOutput.Error($"`{name}` wheel doesn't exist in {cmd.Guild.Name}"), ephemeral:true);
        } else {
            // saves new wheel option(s)
            serverWheels.Add(new Wheel(name, options, serverWheels.Wheels[name].Image));
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Success", "", ImageHelper.Images["Success"])
                .WithTitle($"Values added to `{name}`")
                .WithThumbnail(ImageHelper.Images["SmilingGus"])
                .WithDescription("")
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Green);

            foreach (var o in options) {
                response.Description += $"{o}\n";
            }

            await cmd.CreateResponseAsync(response, ephemeral:true);
        }
    }

    [SlashCommand("spin", "Spins the chosen wheel for a value")]
    public async Task SpinWheel(InteractionContext cmd, 
        [Option("name", "Name of the wheel picker")] string name) {
        
        var serverWheels = new WheelPickers(cmd); 
        
        if (!serverWheels.Contains(name)) {
            // error: wheel doesn't exist
            await cmd.CreateResponseAsync(StandardOutput.Error($"`{name}` wheel doesn't exist in {cmd.Guild.Name}"), ephemeral:true);
        } else {
            // outputs result
            var response = new DiscordEmbedBuilder()
                .WithAuthor($"{cmd.User.GlobalName} spins {name}...", "", serverWheels.Wheels[name].Image)
                .WithTitle(serverWheels.Wheels[name].Spin())
                .WithThumbnail(ImageHelper.Images["PS2Jesse"])
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Gold);

            await cmd.CreateResponseAsync(response);
        }
    }

    [SlashCommand("delete", "Deletes a wheel picker or option from a wheel picker")]
    public async Task DeleteWheel(InteractionContext cmd,
        [Option("name", "Name of the wheel to delete or delete from")] string name,
        [Option("option", "Option to delete from the wheel")] string option="") {
        
        var serverWheels = new WheelPickers(cmd);

        if (!serverWheels.Contains(name)) {
            // error: wheel doesn't exist
            await cmd.CreateResponseAsync(StandardOutput.Error($"`{name}` wheel doesn't exist in {cmd.Guild.Name}"), ephemeral:true);
        } else {
            if (option == "") {
                // delete wheel
                serverWheels.Delete(serverWheels.Wheels[name]);
                
                await cmd.CreateResponseAsync(StandardOutput.Success($"`{name}` deleted from {cmd.Guild.Name}"));
            } else {
                if (serverWheels.Wheels[name].Options.Contains(option)) {
                    // delete option
                    serverWheels.Delete(serverWheels.Wheels[name], option);

                    await cmd.CreateResponseAsync(StandardOutput.Success($"\"{option}\" deleted from `{name}`"), ephemeral:true);
                } else {
                    // error: option doesn't exist in wheel
                    await cmd.CreateResponseAsync(StandardOutput.Error($"\"{option}\" doesn't exist in `{name}`"), ephemeral:true);
                }
            }
        }
    }

    [SlashCommand("list", "Shows all wheels or options in a specific wheel")]
    public async Task ListWheelOptions(InteractionContext cmd, 
        [Option("name", "Name of the wheel picker")] string name="") {

        var serverWheels = new WheelPickers(cmd);

        if (name == "") {
            // display all wheels in server
            if (serverWheels.Wheels.Count == 0) {
                // error: no wheels in server
                await cmd.CreateResponseAsync(StandardOutput.Error($"There are no wheels in {cmd.Guild.Name}"), ephemeral:true);
            } else {
                // displays all wheels
                var response = new DiscordEmbedBuilder()
                    .WithAuthor(cmd.Guild.Name, "", cmd.Guild.IconUrl)
                    .WithTitle("Wheel Pickers")
                    .WithDescription("")
                    .WithThumbnail(ImageHelper.Images["PS2Jesse"])
                    .WithTimestamp(DateTimeOffset.Now)
                    .WithColor(DiscordColor.Blurple);

                var wheelNum = 1;
                foreach (var w in serverWheels.List()) {
                    response.Description += $"`{wheelNum}.` {w.Key} ({w.Value} options)\n";
                    wheelNum++;
                }

                await cmd.CreateResponseAsync(response);
            }
        } else {
            if (!serverWheels.Contains(name)) {
                // error: wheel doesn't exist
                await cmd.CreateResponseAsync(StandardOutput.Error($"`{name}` wheel doesn't exist in {cmd.Guild.Name}"), ephemeral:true);
            } else {
                // display wheel options
                var response = new DiscordEmbedBuilder()
                    .WithAuthor(cmd.Guild.Name, "", cmd.Guild.IconUrl)
                    .WithTitle(serverWheels.Wheels[name].Name)
                    .WithThumbnail(serverWheels.Wheels[name].Image)
                    .WithDescription("")
                    .WithTimestamp(DateTimeOffset.Now)
                    .WithColor(DiscordColor.MidnightBlue);

                var optNum = 1;
                foreach (var o in serverWheels.Wheels[name].Options) {
                    response.Description += $"`{optNum}`. {o}\n";
                    optNum++;
                }

                await cmd.CreateResponseAsync(response);
            }
        }
    }
}

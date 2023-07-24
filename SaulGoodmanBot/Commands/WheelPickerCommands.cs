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
        
        WheelPicker wheel;
        if (img == null) {
            wheel = new WheelPicker(cmd.Guild, name, null, new List<string>(){value1, value2});
        } else {
            wheel = new WheelPicker(cmd.Guild, name, img.Url, new List<string>(){value1, value2});
        }
        
        if (wheel.Exists()) {
            // error: wheel already exists
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle($"'{name}' wheel already exists in {cmd.Guild.Name}")
                .WithColor(DiscordColor.Red)
                .WithThumbnail(ImageHelper.Images["Finger"]);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        } else {
            // saves new wheel
            wheel.SaveWheel();
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Success", "", ImageHelper.Images["Success"])
                .WithTitle($"{name} wheel added")
                .WithThumbnail(ImageHelper.Images["SmilingGus"])
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Green);

            await cmd.CreateResponseAsync(response);
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

        var optionsInput = new List<string>() {option, option2, option3, option4, option5, option6, option7, option8, option9, option10};
        var options = optionsInput.Where(o => o != "").ToList();

        var wheel = new WheelPicker(cmd.Guild, name, null, options);

        if (!wheel.Exists()) {
            // error: wheel doesn't exist
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle($"`{name}` wheel doesn't exist in {cmd.Guild.Name}")
                .WithColor(DiscordColor.Red)
                .WithThumbnail(ImageHelper.Images["Finger"]);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        } else {
            // saves new wheel option
            wheel.SaveWheel();
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Success", "", ImageHelper.Images["Success"])
                .WithTitle($"Values added to `{name}`")
                .WithThumbnail(ImageHelper.Images["SmilingGus"])
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Green);

            await cmd.CreateResponseAsync(response, ephemeral:true);
        }
    }

    [SlashCommand("spin", "Spins the chosen wheel for a value")]
    public async Task SpinWheel(InteractionContext cmd, 
        [Option("name", "Name of the wheel picker")] string name) {
        var wheel = new WheelPicker(cmd.Guild, name);
        
        if (!wheel.Exists()) {
            // error: wheel doesn't exist
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle($"`{name}` wheel doesn't exist in {cmd.Guild.Name}")
                .WithColor(DiscordColor.Red)
                .WithThumbnail(ImageHelper.Images["Finger"]);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        } else {
            // spins the wheel for a value
            var result = wheel.Spin();
            
            // outputs result
            var response = new DiscordEmbedBuilder()
                .WithAuthor($"{cmd.User.GlobalName} spins {name}...", "", wheel.GetImgUrl())
                .WithTitle(result)
                .WithThumbnail(ImageHelper.Images["PS2Jesse"])
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Gold);

            await cmd.CreateResponseAsync(response);
        }
    }

    [SlashCommand("deleteopt", "Deletes an existing option from a wheel")]
    public async Task DeleteWheelOption(InteractionContext cmd,
        [Option("name", "Name of the wheel to delete from")] string name,
        [Option("option", "Option to delete from the wheel")] string option) {
        var wheel = new WheelPicker(cmd.Guild, name);

        if (wheel.DeleteOption(option)) {
            // option successfully deleted
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Success", "", ImageHelper.Images["Success"])
                .WithTitle($"\"{option}\" deleted from `{name}`")
                .WithThumbnail(ImageHelper.Images["SmilingGus"])
                .WithColor(DiscordColor.Green);
            
            await cmd.CreateResponseAsync(response, ephemeral:true);
        } else {
            // wheel or option doesn't exist
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle("Invalid wheel option or invalid wheel")
                .WithThumbnail(ImageHelper.Images["Finger"])
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Red);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        }
    }

    [SlashCommand("delete", "Deletes an existing wheel picker")]
    public async Task DeleteWheel(InteractionContext cmd,
        [Option("name", "Name of the wheel to delete")] string name) {
        var wheel = new WheelPicker(cmd.Guild, name);

        if (!wheel.Exists()) {
            // error: wheel doesn't exist
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle($"`{name}` wheel doesn't exist in {cmd.Guild.Name}")
                .WithColor(DiscordColor.Red)
                .WithThumbnail(ImageHelper.Images["Finger"]);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        } else {
            // wheel successfully deleted
            var response = new DiscordEmbedBuilder()
                .WithAuthor("Success", "", ImageHelper.Images["Success"])
                .WithTitle($"`{name}` deleted from {cmd.Guild.Name}")
                .WithThumbnail(ImageHelper.Images["SmilingGus"])
                .WithColor(DiscordColor.Green);
            
            await cmd.CreateResponseAsync(response);
        }
    }

    [SlashCommand("list", "Shows options in a wheel")]
    public async Task ListWheelOptions(InteractionContext cmd, 
        [Option("name", "Name of the wheel picker")] string name) {

        var wheel = new WheelPicker(cmd.Guild, name);

        if (!wheel.Exists()) {
            // error: wheel doesn't exist
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle($"`{name}` wheel doesn't exist in {cmd.Guild.Name}")
                .WithColor(DiscordColor.Red)
                .WithThumbnail(ImageHelper.Images["Finger"]);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        } else {
            var options = wheel.GetOptions();
            int optionNum = 1;
            
            // outputs wheel options
            var response = new DiscordEmbedBuilder() {
                Title = name,
                Description = "",
                Color = DiscordColor.MidnightBlue
            };

            foreach (var option in options) {
                response.Description += $"{optionNum}: {option}\n";
                optionNum++;
            }

            await cmd.CreateResponseAsync(response);
        }
    }

    [SlashCommand("listall", "Shows all available wheel pickers")]
    public async Task ListWheels(InteractionContext cmd) {
        var wheels = new WheelPicker(cmd.Guild);
        var output = wheels.GetAllWheels();

        if (output == null) {
            // error: no wheels in server
            var error = new DiscordEmbedBuilder()
                .WithAuthor("Error", "", ImageHelper.Images["Error"])
                .WithTitle($"There are no wheels in {cmd.Guild.Name}")
                .WithColor(DiscordColor.Red)
                .WithThumbnail(ImageHelper.Images["Finger"]);

            await cmd.CreateResponseAsync(error, ephemeral:true);
        } else {
            // outputs wheel options
            var response = new DiscordEmbedBuilder()
                .WithAuthor(cmd.Guild.Name, "", cmd.Guild.IconUrl)
                .WithTitle("Wheel Pickers")
                .WithDescription("")
                .WithThumbnail(ImageHelper.Images["PS2Jesse"])
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(DiscordColor.Blurple);

            var wheelNum = 1;
            foreach (var wheel in output) {
                response.Description += $"`{wheelNum}.` {wheel.Key} ({wheel.Value} options)\n";
                wheelNum++;
            }

            await cmd.CreateResponseAsync(response);
        }
    }
}

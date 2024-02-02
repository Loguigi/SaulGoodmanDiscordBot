/*
    StandardOutput.cs

    Static class for standard embed message output, like success and error messages
*/

using DSharpPlus;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Helpers;

public static class StandardOutput {
    public static DiscordEmbedBuilder Success(string message) {
        return new DiscordEmbedBuilder()
            .WithAuthor("Success", "", ImageHelper.Images["Success"])
            .WithDescription(message)
            .WithThumbnail(ImageHelper.Images["SmilingGus"])
            .WithColor(DiscordColor.Green);
    }

    public static DiscordEmbedBuilder Error(string message) {
        return new DiscordEmbedBuilder()
            .WithAuthor("Error", "", ImageHelper.Images["Error"])
            .WithDescription(message)
            .WithColor(DiscordColor.Red)
            .WithThumbnail(ImageHelper.Images["Finger"]);
    }

    public static DiscordEmbedBuilder ConfigChange(DiscordClient client, DiscordGuild guild, ConfigChangeOption option, string config_name, string oldMsg="", string newMsg="") {
        return new DiscordEmbedBuilder()
            .WithAuthor("Config Change", "", guild.IconUrl)
            .WithTitle(config_name)
            .WithColor(DiscordColor.Azure)
            .WithDescription(option switch {
                ConfigChangeOption.EnabledToDisabled => $"### {DiscordEmoji.FromName(client, ":white_check_mark:")} Enabled {DiscordEmoji.FromName(client, ":arrow_right:")} {DiscordEmoji.FromName(client, ":x:")} Disabled",
                ConfigChangeOption.DisabledToEnabled => $"### {DiscordEmoji.FromName(client, ":x:")} Disabled {DiscordEmoji.FromName(client, ":arrow_right:")} {DiscordEmoji.FromName(client, ":white_check_mark:")} Enabled",
                ConfigChangeOption.MessageChange => $"### {oldMsg} {DiscordEmoji.FromName(client, ":arrow_right:")} {newMsg}",
                _ => ""
            });
    }
}
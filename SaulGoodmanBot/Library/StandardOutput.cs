/*
    StandardOutput.cs

    Static class for standard embed message output, like success and error messages
*/

using DSharpPlus.Entities;

namespace SaulGoodmanBot.Library;

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
}
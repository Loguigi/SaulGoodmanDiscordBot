using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandPermissions(Permissions.Administrator)]
[SlashCommandGroup("config", "Bot configuration commands")]
public class ServerConfigCommands : ApplicationCommandModule {
    [SlashCommand("messags", "Bot message responses to events")]
    public async Task GeneralConfig(InteractionContext ctx,
        [Choice("Welcome message", "welcome")]
        [Choice("Leave message", "leave")]
        [Choice("Birthday message", "birthday")]
        [Choice("Level up message", "levelup")]
        [Option("option", "General config option")] string option) {
            
        var config = new ServerConfig(ctx.Guild);
        var intr = ctx.Client.GetInteractivity();

        // Welcome Message
        if (option == "welcome") {
            var prompt = new DiscordEmbedBuilder()
                .WithTitle("Set Welcome Message")
                .WithDescription("Enter a welcome message for your server\nFormat is `[message] @user`\nEnter `cancel` to cancel or `disable` to disable welcome messages");
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(prompt)));

            var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
            if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                // cancel operation
                prompt.Description = "Operation cancelled";
            } else if (response.Result.Content.ToLower().Contains("disable")) {
                // disable welcome message
                prompt.Description = "Welcome message disabled";
                config.WelcomeMessage = null;
            } else {
                // save new welcome message
                prompt.Description = "Welcome message set";
                config.WelcomeMessage = response.Result.Content;
            }

            config.UpdateConfig();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(prompt));
            await ctx.Channel.DeleteMessageAsync(response.Result);
        
        // Leave Message
        } else if (option == "leave") {
            var prompt = new DiscordEmbedBuilder()
                .WithTitle("Set Leave Message")
                .WithDescription("Enter a leave message for your server\nFormat is `@user [message]`\nEnter `cancel` to cancel or `disable` to disable leave messages");
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(prompt)));

            var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
            if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                // cancel operation
                prompt.Description = "Operation cancelled";
            } else if (response.Result.Content.ToLower().Contains("disable")) {
                // disable leave message
                prompt.Description = "Leave message disabled";
                config.LeaveMessage = null;
            } else {
                // save new leave message
                prompt.Description = "Leave message set";
                config.LeaveMessage = response.Result.Content;
            }

            config.UpdateConfig();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(prompt));
            await ctx.Channel.DeleteMessageAsync(response.Result);

        // Birthday Message
        } else if (option == "birthday") {
            var prompt = new DiscordEmbedBuilder()
                .WithTitle("Set Birthday Message")
                .WithDescription("Enter a birthday message for your server\nFormat is `[message] @user`\nEnter `cancel` to cancel, `disable` to disable birthday messages, or `default` to reset the message");
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(prompt)));

            var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
            if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                // cancel operation
                prompt.Description = "Operation cancelled";
            } else if (response.Result.Content.ToLower().Contains("disable")) {
                // disable birthday message
                prompt.Description = "Birthday messages disabled";
                config.BirthdayNotifications = false;
            } else if (response.Result.Content.ToLower().Contains("default")) {
                // reset back to default
                prompt.Description = "Birthday message set";
                config.BirthdayNotifications = true;
                config.BirthdayMessage = "Happy Birthday!";
            } else {
                // save new birthday message
                prompt.Description = "Birthday message set";
                config.BirthdayNotifications = true;
                config.BirthdayMessage = response.Result.Content;
            }

            config.UpdateConfig();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(prompt));
            await ctx.Channel.DeleteMessageAsync(response.Result);

        // Level Up Message
        } else if (option == "levelup") {
            var prompt = new DiscordEmbedBuilder()
                .WithTitle("Set Level Up Message")
                .WithDescription("Enter a level up message for your server\nFormat is `@user [message]`\nEnter `cancel` to cancel or `default` to reset the message");
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(prompt)));

            var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
            if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                // cancel operation
                prompt.Description = "Operation cancelled";
            } else if (response.Result.Content.ToLower().Contains("default")) {
                // reset back to default
                prompt.Description = "Level up message set";
                config.LevelUpMessage = "has levelled up!";
            } else {
                // save new level up message
                prompt.Description = "Level up message set";
                config.LevelUpMessage = response.Result.Content;
            }

            config.UpdateConfig();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(prompt));
            await ctx.Channel.DeleteMessageAsync(response.Result);
        }
    }

    [SlashCommand("defaultchannel", "Chooses a default channel to send messages in")]
    public async Task DefaultChannel(InteractionContext ctx,
        [Option("channel", "Channel to select as default")] DiscordChannel channel) {

        var config = new ServerConfig(ctx.Guild) {DefaultChannel = channel};
        config.UpdateConfig();

        await ctx.CreateResponseAsync(StandardOutput.Success($"Default channel set to {channel.Mention}"), ephemeral:true);
    }

    [SlashCommand("levels", "Enable or disable levels")]
    public async Task LevelsConfig (InteractionContext ctx,
        [Choice("Enable", "enable")]
        [Choice("Disable", "disable")]
        [Option("option", "Option")] string option) {
        
        var config = new ServerConfig(ctx.Guild);
        if (option == "enable") {
            config.EnableLevels = true;
        } else if (option == "disable") {
            config.EnableLevels = false;
        }
        config.UpdateConfig();

        await ctx.CreateResponseAsync(StandardOutput.Success($"Levels {(config.EnableLevels ? "enabled" : "disabled")} in {ctx.Guild.Name}"));
    }
}
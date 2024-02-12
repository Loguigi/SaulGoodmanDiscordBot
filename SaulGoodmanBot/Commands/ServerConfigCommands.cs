using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Helpers;
using DSharpPlus.SlashCommands.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashRequirePermissions(Permissions.Administrator)]
[SlashCommandGroup("config", "Bot configuration commands")]
public class ServerConfigCommands : ApplicationCommandModule {
    [SlashCommand("messages", "Bot message responses to events")]
    public async Task MessageConfig(InteractionContext ctx,
        [Choice("Welcome message", "welcome")]
        [Choice("Leave message", "leave")]
        [Choice("Birthday message", "birthday")]
        [Choice("Level up message", "levelup")]
        [Option("option", "General config option")] string option) {
            
        try {
            var config = new ServerConfig(ctx.Guild);
            var intr = ctx.Client.GetInteractivity();

            // Welcome Message
            if (option == "welcome") {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Set Welcome Message")
                    .WithDescription("Enter a welcome message for your server\nFormat is `[message] @user`\nEnter `cancel` to cancel or `disable` to disable welcome messages");
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

                var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
                if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                    // cancel operation
                    embed.Description = "Operation cancelled";
                } else if (response.Result.Content.ToLower().Contains("disable")) {
                    // disable welcome message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.EnabledToDisabled, "Welcome message");
                    config.WelcomeMessage = null;
                } else {
                    // save new welcome message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Welcome message", config.WelcomeMessage ?? "", response.Result.Content);
                    config.WelcomeMessage = response.Result.Content;
                }

                config.Save();
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                await ctx.Channel.DeleteMessageAsync(response.Result);
            
            // Leave Message
            } else if (option == "leave") {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Set Leave Message")
                    .WithDescription("Enter a leave message for your server\nFormat is `@user [message]`\nEnter `cancel` to cancel or `disable` to disable leave messages");
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

                var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
                if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                    // cancel operation
                    embed.Description = "Operation cancelled";
                } else if (response.Result.Content.ToLower().Contains("disable")) {
                    // disable leave message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.EnabledToDisabled, "Leave message");
                    config.LeaveMessage = null;
                } else {
                    // save new leave message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Leave message", config.LeaveMessage ?? "", response.Result.Content);
                    config.LeaveMessage = response.Result.Content;
                }

                config.Save();
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                await ctx.Channel.DeleteMessageAsync(response.Result);

            // Birthday Message
            } else if (option == "birthday") {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Set Birthday Message")
                    .WithDescription("Enter a birthday message for your server\nFormat is `[message] @user`\nEnter `cancel` to cancel, `disable` to disable birthday messages, or `default` to reset the message");
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

                var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
                if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                    // cancel operation
                    embed.Description = "Operation cancelled";
                } else if (response.Result.Content.ToLower().Contains("disable")) {
                    // disable birthday message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.EnabledToDisabled, "Birthday notifications");
                    config.BirthdayNotifications = false;
                } else if (response.Result.Content.ToLower().Contains("default")) {
                    // reset back to default
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Birthday message", config.BirthdayMessage, "Happy Birthday!");
                    config.BirthdayNotifications = true;
                    config.BirthdayMessage = "Happy Birthday!";
                } else {
                    // save new birthday message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Birthday message", config.BirthdayMessage, response.Result.Content);
                    config.BirthdayNotifications = true;
                    config.BirthdayMessage = response.Result.Content;
                }

                config.Save();
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                await ctx.Channel.DeleteMessageAsync(response.Result);

            // Level Up Message
            } else if (option == "levelup") {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Set Level Up Message")
                    .WithDescription("Enter a level up message for your server\nFormat is `@user [message]`\nEnter `cancel` to cancel or `default` to reset the message");
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

                var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
                if (response.Result.Content.ToLower().Contains("cancel") || response.TimedOut) {
                    // cancel operation
                    embed.Description = "Operation cancelled";
                } else if (response.Result.Content.ToLower().Contains("default")) {
                    // reset back to default
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Level up message", config.LevelUpMessage, "has levelled up!");
                    config.LevelUpMessage = "has levelled up!";
                } else {
                    // save new level up message
                    embed = StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Level up message", config.LevelUpMessage, response.Result.Content);
                    config.LevelUpMessage = response.Result.Content;
                }

                config.Save();
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                await ctx.Channel.DeleteMessageAsync(response.Result);
            }
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("defaultchannel", "Chooses a default channel to send messages in")]
    public async Task DefaultChannel(InteractionContext ctx,
        [Option("channel", "Channel to select as default")] DiscordChannel channel) {
        try {
            var config = new ServerConfig(ctx.Guild) {DefaultChannel = channel};
            config.Save();

            await ctx.CreateResponseAsync(StandardOutput.ConfigChange(ctx.Client, ctx.Guild, ConfigChangeOption.MessageChange, "Default channel changed", "", channel.Mention));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("levels", "Enable or disable levels")]
    public async Task LevelsConfig (InteractionContext ctx,
        [Choice("Enable", "enable")]
        [Choice("Disable", "disable")]
        [Option("option", "Option")] string option) {

        try {
            var config = new ServerConfig(ctx.Guild) {
                EnableLevels = option == "enable"
            };
            config.Save();

            await ctx.CreateResponseAsync(StandardOutput.ConfigChange(ctx.Client, ctx.Guild, config.EnableLevels ? ConfigChangeOption.DisabledToEnabled : ConfigChangeOption.EnabledToDisabled, "Server levels"));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("birthday_notifications", "Enable or disable all birthday notifications")]
    public async Task BirthdayNotifications(InteractionContext ctx,
        [Choice("Enable", "enable")]
        [Choice("Disable", "disable")]
        [Option("option", "Option")] string option) {
        
        try {
            var config = new ServerConfig(ctx.Guild) {
                BirthdayNotifications = option == "enable"
            };
            config.Save();

            await ctx.CreateResponseAsync(StandardOutput.ConfigChange(ctx.Client, ctx.Guild, config.BirthdayNotifications ? ConfigChangeOption.DisabledToEnabled : ConfigChangeOption.EnabledToDisabled, "Birthday notifications"));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("role_menu", "Enable or disable the sending of role menu when a member joins the server")]
    public async Task RoleMenuOnMemberJoin(InteractionContext ctx,
        [Choice("Enable", "enable")]
        [Choice("Disable", "disable")]
        [Option("option", "Option")] string option) {
        
        try {
            var config = new ServerConfig(ctx.Guild) {
                SendRoleMenuOnMemberJoin = option == "enable"
            };
            config.Save();

            await ctx.CreateResponseAsync(StandardOutput.ConfigChange(ctx.Client, ctx.Guild, config.SendRoleMenuOnMemberJoin ? ConfigChangeOption.DisabledToEnabled : ConfigChangeOption.EnabledToDisabled, "Send role menu on member join"));
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
}
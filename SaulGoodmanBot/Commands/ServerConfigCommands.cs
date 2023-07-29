using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandPermissions(Permissions.Administrator)]
[SlashCommandGroup("config", "Bot configuration commands")]
public class ServerConfigCommands : ApplicationCommandModule {
    [SlashCommand("general", "General options like welcome and leave message")]
    public async Task GeneralConfig(InteractionContext ctx,
        [Choice("Welcome message", "welcome")]
        [Choice("Leave message", "leave")]
        [Option("option", "General config option")] string option) {
        var config = new ServerConfig(ctx.Guild.Id);
        var intr = ctx.Client.GetInteractivity();
        var description = String.Empty;

        if (option == "welcome") {
            description = "Enter a welcome message for your server\nFormat is `[message] @user`\nEnter `cancel` to cancel or `disable` to disable welcome messages";
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Set Welcome Message")
                    .WithDescription(description));
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
            if (response.Result.Content.ToLower().Contains("cancel")) {
                // cancel operation
                description = "Operation cancelled";
            } else if (response.Result.Content.ToLower().Contains("disable")) {
                // disable welcome message
                description = "Welcome message disabled";
                config.WelcomeMessage = null;
                config.UpdateConfig();
            } else {
                // save new welcome message
                description = "Welcome message set";
                config.WelcomeMessage = response.Result.Content;
                config.UpdateConfig();
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder().WithTitle("Set Welcome Message").WithDescription(description)));
            await ctx.Channel.DeleteMessageAsync(response.Result);
        } else if (option == "leave") {
            description = "Enter a leave message for your server\nFormat is `@user [message]`\nEnter `cancel` to cancel or `disable` to disable leave messages";
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Set Leave Message")
                    .WithDescription(description));
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            var response = await intr.WaitForMessageAsync(u => u.Author == ctx.Member, TimeSpan.FromSeconds(60));
            if (response.Result.Content.ToLower().Contains("cancel")) {
                // cancel operation
                description = "Operation cancelled";
            } else if (response.Result.Content.ToLower().Contains("disable")) {
                // disable leave message
                description = "Leave message disabled";
                config.LeaveMessage = null;
                config.UpdateConfig();
            } else {
                // save new leave message
                description = "Leave message set";
                config.WelcomeMessage = response.Result.Content;
                config.UpdateConfig();
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder().WithTitle("Set Leave Message").WithDescription(description)));
            await ctx.Channel.DeleteMessageAsync(response.Result);
        }
    } 
}
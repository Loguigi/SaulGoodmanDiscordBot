using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class GeneralHandlers {
    public static async Task HandleOnReady(DiscordClient s, SessionReadyEventArgs e) {
        var activity = new DiscordActivity("you're not a real lawyer");
        await s.UpdateStatusAsync(activity);
    }

    public static async Task HandleMemberJoin(DiscordClient s, GuildMemberAddEventArgs e) {
        var config = new ServerConfig(e.Guild);
        if (config.WelcomeMessage != null) {
            _ = await new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription($"## {config.WelcomeMessage} {e.Member.Mention}")
                    .WithColor(DiscordColor.Gold))
                .SendAsync(config.DefaultChannel);
        }
    }

    public static async Task HandleMemberLeave(DiscordClient s, GuildMemberRemoveEventArgs e) {
        var config = new ServerConfig(e.Guild);
        if (config.LeaveMessage != null) {
            _ = await new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription($"## {e.Member.Mention} {config.LeaveMessage}")
                    .WithColor(DiscordColor.Orange))
                .SendAsync(config.DefaultChannel);
        }
    }
}
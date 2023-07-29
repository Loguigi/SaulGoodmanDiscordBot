using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class GeneralHandlers {
    public static async Task HandleOnReady(DiscordClient s, SessionReadyEventArgs e) {
        var activity = new DiscordActivity("testing");
        await s.UpdateStatusAsync(activity, UserStatus.DoNotDisturb);
    }

    public static async Task HandleMemberJoin(DiscordClient s, GuildMemberAddEventArgs e) {
        var config = new ServerConfig(e.Guild.Id);
        if (config.WelcomeMessage != null) {
            var message = await new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription($"## {config.WelcomeMessage} {e.Member.Mention}")
                    .WithColor(DiscordColor.Gold))
                .SendAsync(e.Guild.GetDefaultChannel());
        }
    }

    public static async Task HandleMemberLeave(DiscordClient s, GuildMemberRemoveEventArgs e) {
        var config = new ServerConfig(e.Guild.Id);
        if (config.LeaveMessage != null) {
            var message = await new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription($"## {e.Member.Mention} {config.LeaveMessage}")
                    .WithColor(DiscordColor.Orange))
                .SendAsync(e.Guild.GetDefaultChannel());
        }
    }
}
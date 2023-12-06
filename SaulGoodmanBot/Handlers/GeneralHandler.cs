using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Birthdays;
using SaulGoodmanBot.Commands;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class GeneralHandlers {
    public static async Task HandleOnReady(DiscordClient s, SessionReadyEventArgs e) {
        var activity = new DiscordActivity("Garry's Mod", ActivityType.Streaming);
        await s.UpdateStatusAsync(activity, UserStatus.DoNotDisturb);
    }

    public static async Task HandleMemberJoin(DiscordClient s, GuildMemberAddEventArgs e) {
        var config = new ServerConfig(e.Guild);
        if (config.WelcomeMessage == null) {
            await Task.CompletedTask;
            return;
        }

        _ = await new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithDescription($"## {config.WelcomeMessage} {e.Member.Mention}")
                .WithColor(DiscordColor.Green))
            .SendAsync(config.DefaultChannel);
    }

    public static async Task HandleMemberLeave(DiscordClient s, GuildMemberRemoveEventArgs e) {
        var config = new ServerConfig(e.Guild);
        var birthdays = new ServerBirthdays(e.Guild);

        birthdays.Edit(DataOperations.Delete, birthdays.Find(e.Member));
        
        if (config.LeaveMessage == null) {
            await Task.CompletedTask;
            return;
        }

        _ = await new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithDescription($"## {e.Member.Mention} {config.LeaveMessage}")
                .WithColor(DiscordColor.Red))
            .SendAsync(config.DefaultChannel);
    }

    public static async Task HandleServerJoin(DiscordClient s, GuildCreateEventArgs e) {
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Saul Goodman", "", ImageHelper.Images["Heisenberg"])
            .WithTitle(HelpText.Setup.First().Key)
            .WithDescription(HelpText.Setup.First().Value)
            .WithThumbnail(ImageHelper.Images["Saul"])
            .WithColor(DiscordColor.Orange);
        
        var pages = new List<DiscordSelectComponentOption>();
        foreach (var p in HelpText.Setup.Keys) {
            pages.Add(new DiscordSelectComponentOption(p, p));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.Help.SETUP, "Select a page...", pages);

        await e.Guild.GetDefaultChannel().SendMessageAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
        await e.Guild.GetDefaultChannel().SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown));

        s.ComponentInteractionCreated -= HelpHandler.HandleSetupHelp;
        s.ComponentInteractionCreated += HelpHandler.HandleSetupHelp;
    }
}
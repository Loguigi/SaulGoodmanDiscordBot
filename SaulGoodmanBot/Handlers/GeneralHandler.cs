using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class GeneralHandlers {

    public static async Task HandleMemberJoin(DiscordClient s, GuildMemberAddEventArgs e) 
    {
        var config = new ServerConfig(e.Guild);
        var member = new ServerMember(e.Guild, e.Member);
        await member.Activate();
        
        if (config.WelcomeMessage == null) 
        {
            await Task.CompletedTask;
            return;
        }

        _ = await new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithDescription($"## {config.WelcomeMessage} {e.Member.Mention}")
                .WithColor(DiscordColor.Green))
            .SendAsync(config.DefaultChannel);

        var roles = new ServerRoles(e.Guild);
        if (roles.IsNotSetup && !config.SendRoleMenuOnMemberJoin) 
        {
            await Task.CompletedTask;
            return;
        }

        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles.Roles) 
        {
            roleOptions.Add(new DiscordSelectComponentOption(r.Role.Name, r.Role.Id.ToString(), r.Description));
        }
        DiscordSelectComponent roleDropdown = roles.AllowMultipleRoles ? new(IDHelper.Roles.MENU, "Select roles", roleOptions, false, 1, roleOptions.Count) : new(IDHelper.Roles.MENU, "Select a role", roleOptions, false);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "https://youtu.be/dQw4w9WgXcQ", e.Guild.IconUrl)
            .WithTitle(roles.CategoryName)
            .WithDescription(roles.CategoryDescription)
            .WithFooter(roles.AllowMultipleRoles ? "Can have multiple" : "Can only have one")
            .WithColor(DiscordColor.Turquoise);

        foreach (var r in roles.Roles) 
        {
            if (r == roles.Roles.First())
                embed.AddField("Available Roles", r.Role.Mention);
            else
                embed.Fields.First(x => x.Name == "Available Roles").Value += $", {r.Role.Mention}";
        }
        await config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent(e.Member.Mention).AddMention(new UserMention(e.Member)).AddEmbed(embed).AddComponents(roleDropdown));

        // Add handler
        s.ComponentInteractionCreated -= RoleHandler.HandleMenu;
        s.ComponentInteractionCreated += RoleHandler.HandleMenu;
    }

    public static async Task HandleMemberLeave(DiscordClient s, GuildMemberRemoveEventArgs e) 
    {
        var config = new ServerConfig(e.Guild);
        var member = new ServerMember(e.Guild, e.Member);
        await member.Deactivate();
        
        if (config.LeaveMessage == null) 
        {
            await Task.CompletedTask;
            return;
        }

        _ = await new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithDescription($"## {e.Member.Mention} {config.LeaveMessage}")
                .WithColor(DiscordColor.Red))
            .SendAsync(config.DefaultChannel);
    }

    public static async Task HandleServerJoin(DiscordClient s, GuildCreateEventArgs e) 
    {
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Saul Goodman", "", ImageHelper.Images["Heisenberg"])
            .WithTitle(HelpText.Setup.First().Key)
            .WithDescription(HelpText.Setup.First().Value)
            .WithThumbnail(ImageHelper.Images["Saul"])
            .WithColor(DiscordColor.Orange);
        
        var pages = new List<DiscordSelectComponentOption>();
        foreach (var p in HelpText.Setup.Keys) 
        {
            pages.Add(new DiscordSelectComponentOption(p, p));
        }
        var dropdown = new DiscordSelectComponent(IDHelper.Help.SETUP, "Select a page...", pages);

        await e.Guild.GetDefaultChannel().SendMessageAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
        await e.Guild.GetDefaultChannel().SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown));

        // s.ComponentInteractionCreated -= HelpHandler.HandleSetupHelp;
        // s.ComponentInteractionCreated += HelpHandler.HandleSetupHelp;
    }
}
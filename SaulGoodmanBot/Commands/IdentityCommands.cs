using Dapper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Handlers;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
public class IdentityCommands : ApplicationCommandModule
{
    [SlashCommand("who", "Lists names of users in the server")]
    public async Task Who(InteractionContext ctx)
    {
        var identities = new Identities(ctx.Guild).GetInteractivity();
        
        if (identities.GetPage().Count == 0)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error("No users have their name saved"), ephemeral: true);
            return;
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Who are you?")
            .WithColor(DiscordColor.Aquamarine)
            .WithDescription("")
            .WithAuthor(ctx.Guild.Name, null, ctx.Guild.IconUrl)
            .WithFooter(identities.PageStatus);

        foreach (var member in identities.GetPage())
        {
            embed.Description += $"### * {member.User.Mention} => {member.Name}\n";
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(identities.AddPageButtons().AddEmbed(embed)));

        ctx.Client.ComponentInteractionCreated -= IdentityHandler.HandleWhoList;
        ctx.Client.ComponentInteractionCreated += IdentityHandler.HandleWhoList;
    }
    
    [ContextMenu(ApplicationCommandType.UserContextMenu, "What's your name?")]
    public async Task ContextCheckWho(ContextMenuContext ctx) 
    {
        if (ctx.TargetUser.IsBot) 
        {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        var identity = new ServerMember(ctx.Guild, ctx.TargetUser);

        if (identity.Name is null)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error($"{ctx.TargetUser.Mention} does not have a name set"), ephemeral: true);
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Who are you?")
            .WithColor(DiscordColor.Aquamarine)
            .WithDescription($"### **{identity.Name}**")
            .WithAuthor(ctx.TargetUser.GlobalName, null, ctx.TargetUser.AvatarUrl);

        await ctx.CreateResponseAsync(embed, ephemeral: true);
    }

    [SlashCommand("iam", "Specify your own or somebody else's name")]
    public async Task IAm(InteractionContext ctx,
        [Option("name", "Your Name")][MinimumLength(1)][MaximumLength(100)] string name, 
        [Option("user", "User to change the name of")] DiscordUser? user=null)
    {
        var identity = new ServerMember(ctx.Guild, user ?? ctx.User);
        
        identity.Name = name;
        await identity.Save();

        if (user is null)
        {
            await ctx.CreateResponseAsync(StandardOutput.Success("Your name has been changed!"), ephemeral: true);
        }
        else
        {
            await ctx.CreateResponseAsync(StandardOutput.Success($"{user.Mention}'s name has been changed!"), ephemeral: true);
        }
    }
    
    [SlashCommand("whois", "Get the name of somebody in the server")]
    public async Task WhoIs(InteractionContext ctx,
        [Option("user", "User to check the name of")] DiscordUser user) 
    {
        if (user.IsBot) 
        {
            await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
            return;
        }

        var identity = new ServerMember(ctx.Guild, user);

        if (identity.Name is null)
        {
            await ctx.CreateResponseAsync(StandardOutput.Error($"{user.Mention} does not have a name set"), ephemeral: true);
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Who are you?")
            .WithColor(DiscordColor.Aquamarine)
            .WithDescription($"# **{identity.Name}**")
            .WithAuthor(user.GlobalName, null, user.AvatarUrl);

        await ctx.CreateResponseAsync(embed, ephemeral: true);
    }
}
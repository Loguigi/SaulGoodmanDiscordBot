using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Handlers;
using DSharpPlus.SlashCommands.Attributes;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("role", "Manage self-assigning roles in server")]
public class RoleCommands : ApplicationCommandModule 
{
    [SlashCommand("setup", "Setup how roles are assigned in your server")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task Setup(InteractionContext ctx,
        [Option("name", "Name for the category of roles")] string name,
        [Option("description", "Description for the role category")] string description,
        [Option("allowmultipleroles", "Allows multiple roles to be assigned")] bool allowmultipleroles) 
    {
        var config = new ServerConfig(ctx.Guild) 
        {
            ServerRolesName = name,
            ServerRolesDescription = description,
            AllowMultipleRoles = allowmultipleroles
        };
        await config.Save();

        await ctx.CreateResponseAsync(StandardOutput.Success("Role setup complete. Add roles to finish using /role add"), ephemeral:true);
    }

    [SlashCommand("add", "Add roles to the list of self-assignable roles")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task AddRole(InteractionContext ctx,
        [Option("role", "Role to add")] DiscordRole role,
        [Option("description", "Description to add to the role")] string? description=null,
        [Option("emoji", "Emoji to represent the role")] string? emoji=null) 
    {
        var roles = new ServerRoles(ctx.Guild);

        if (roles[role] != null)
            throw new Exception($"{role.Mention} already added to {roles.CategoryName}");

        if (emoji == null) 
        {
            await roles.Add(new ServerRoles.RoleComponent(role, description ?? string.Empty, ServerRoles.DefaultEmoji));
            await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);
            return;
        }

        #region Emoji Parsing
        // Standard unicode emoji
        if (DiscordEmoji.TryFromUnicode(ctx.Client, emoji, out DiscordEmoji normalEmoji)) 
        {
            roles.Add(new ServerRoles.RoleComponent(role, description ?? string.Empty, normalEmoji));
            await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);

            
        } 
        else if (emoji.Contains(':')) // Not standard unicode emoji; possible guild emoji
        {
            var tryEmoji = ":" + string.Join("", emoji.SkipWhile(x => x != ':').Skip(1).TakeWhile(x => x != ':')) + ":";

            if (DiscordEmoji.TryFromName(ctx.Client, tryEmoji, true, out DiscordEmoji guildEmoji)) {
                roles.Add(new ServerRoles.RoleComponent(role, description ?? string.Empty, guildEmoji));
                await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);

            
            } 
            else // ERROR: emoji error
                throw new Exception($"Invalid emoji: {emoji}");

        
        } 
        else // ERROR: emoji error
            throw new Exception($"Invalid emoji: {emoji}");
        #endregion
    }

    [SlashCommand("remove", "Remove a role from the list of self-assignable roles")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task RemoveRole(InteractionContext ctx) 
    {
        var roles = new ServerRoles(ctx.Guild);

        // Create dropdown
        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles.Roles) 
        {
            roleOptions.Add(new DiscordSelectComponentOption(r.Role.Name, r.Role.Id.ToString(), "", false));
        }
        var roleDropdown = new DiscordSelectComponent(IDHelper.Roles.REMOVE, "Select a role", roleOptions);
        var cancelButton = new DiscordButtonComponent(ButtonStyle.Secondary, IDHelper.Roles.REMOVE, "Cancel", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":arrow_left:", false)));

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Remove Role")
            .WithColor(DiscordColor.DarkRed);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(roleDropdown)));

        ctx.Client.ComponentInteractionCreated -= RoleHandler.HandleRemoveRole;
        ctx.Client.ComponentInteractionCreated += RoleHandler.HandleRemoveRole;
    }

    [SlashCommand("menu", "Creates a menu for assigning and unassigning roles")]
    public async Task RoleMenu(InteractionContext ctx) 
    {
        var roles = new ServerRoles(ctx.Guild);
        
        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles.Roles) 
        {
            roleOptions.Add(new DiscordSelectComponentOption(r.Role.Name, r.Role.Id.ToString(), r.Description, false, new DiscordComponentEmoji(r.Emoji)));
        }
        DiscordSelectComponent roleDropdown = roles.AllowMultipleRoles ? new(IDHelper.Roles.MENU, "Select roles", roleOptions, false, 1, roleOptions.Count) : new(IDHelper.Roles.MENU, "Select a role", roleOptions, false);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(ctx.Guild.Name, "https://youtu.be/dQw4w9WgXcQ", ctx.Guild.IconUrl)
            .WithTitle(roles.CategoryName)
            .WithDescription(roles.CategoryDescription)
            .WithFooter(roles.AllowMultipleRoles ? "Can have multiple" : "Can only have one")
            .WithColor(DiscordColor.Turquoise);

        foreach (var r in roles.Roles) 
        {
            if (r == roles.Roles.First())
                embed.AddField("Available Roles", r.Role.Mention);
            else
                embed.Fields.Where(x => x.Name == "Available Roles").First().Value += $", {r.Role.Mention}";
        }
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(roleDropdown)));

        // Add handler
        ctx.Client.ComponentInteractionCreated -= RoleHandler.HandleMenu;
        ctx.Client.ComponentInteractionCreated += RoleHandler.HandleMenu;
    }

    [SlashCommand("assign", "Assign or unassign a role")]
    public async Task AssignRole(InteractionContext ctx) 
    {
        var roles = new ServerRoles(ctx.Guild);

        // Create role options and dropdowns
        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles.Roles) 
        {
            roleOptions.Add(new DiscordSelectComponentOption(r.Role.Name, r.Role.Id.ToString(), r.Description, false, new DiscordComponentEmoji(r.Emoji)));
        }
        var roleDropdown = new DiscordSelectComponent(IDHelper.Roles.ASSIGN, "Select a role", roleOptions);
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle(roles.CategoryName)
            .WithDescription(roles.CategoryDescription)
            .WithColor(DiscordColor.Turquoise);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(roleDropdown)));

        ctx.Client.ComponentInteractionCreated -= RoleHandler.HandleAssign;
        ctx.Client.ComponentInteractionCreated += RoleHandler.HandleAssign;

    }
}

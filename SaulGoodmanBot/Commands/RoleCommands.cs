using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library.Roles;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Helpers;
using SaulGoodmanBot.Handlers;
using DSharpPlus.SlashCommands.Attributes;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("role", "Manage self-assigning roles in server")]
public class RoleCommands : ApplicationCommandModule {
    [SlashCommand("setup", "Setup how roles are assigned in your server")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task Setup(InteractionContext ctx,
        [Option("name", "Name for the category of roles")] string name,
        [Option("description", "Description for the role category")] string description,
        [Option("allowmultipleroles", "Allows multiple roles to be assigned")] bool allowmultipleroles) {
        
        var config = new ServerConfig(ctx.Guild) {
            ServerRolesName = name,
            ServerRolesDescription = description,
            AllowMultipleRoles = allowmultipleroles
        };
        config.UpdateConfig();

        await ctx.CreateResponseAsync(StandardOutput.Success("Role setup complete. Add roles to finish using /role add"), ephemeral:true);
    }

    [SlashCommand("add", "Add roles to the list of self-assignable roles")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task AddRole(InteractionContext ctx,
        [Option("role", "Role to add")] DiscordRole role,
        [Option("description", "Description to add to the role")] string? description=null,
        [Option("emoji", "Emoji to represent the role")] string? emoji=null) {
        
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        if (roles.Contains(role))
            throw new Exception($"{role.Mention} already added to {roles.CategoryName}");

        if (emoji == null) {
            roles.Add(new RoleComponent(role, description ?? string.Empty, roles.DEFAULT_EMOJI));
            await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);
            return;
        }

        #region Emoji Parsing
        // Standard unicode emoji
        if (DiscordEmoji.TryFromUnicode(ctx.Client, emoji, out DiscordEmoji normalEmoji)) {
            roles.Add(new RoleComponent(role, description ?? string.Empty, normalEmoji));
            await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);

        // Not standard unicode emoji; possible guild emoji
        } else if (emoji.Contains(':')) {
            var tryEmoji = ":" + string.Join("", emoji.SkipWhile(x => x != ':').Skip(1).TakeWhile(x => x != ':')) + ":";

            if (DiscordEmoji.TryFromName(ctx.Client, tryEmoji, true, out DiscordEmoji guildEmoji)) {
                roles.Add(new RoleComponent(role, description ?? string.Empty, guildEmoji));
                await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);

            // ERROR: emoji error
            } else
                throw new Exception("Invalid emoji");

        // ERROR: emoji error
        } else 
            throw new Exception("Invalid emoji");
        #endregion
    }

    [SlashCommand("remove", "Remove a role from the list of self-assignable roles")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task RemoveRole(InteractionContext ctx) {
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        // Create dropdown
        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles) {
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
    public async Task RoleMenu(InteractionContext ctx) {
        var roles = new ServerRoles(ctx.Guild, ctx.Client);
        
        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles) {
            roleOptions.Add(new DiscordSelectComponentOption(r.Role.Name, r.Role.Id.ToString(), r.Description, false, new DiscordComponentEmoji(r.Emoji)));
        }
        DiscordSelectComponent roleDropdown = roles.AllowMultipleRoles ? new(IDHelper.Roles.MENU, "Select roles", roleOptions, false, 1, roleOptions.Count) : new(IDHelper.Roles.MENU, "Select a role", roleOptions, false);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(ctx.Guild.Name, "https://youtu.be/dQw4w9WgXcQ", ctx.Guild.IconUrl)
            .WithTitle(roles.CategoryName)
            .WithDescription(roles.CategoryDescription)
            .WithFooter(roles.AllowMultipleRoles ? "Can have multiple" : "Can only have one")
            .WithColor(DiscordColor.Turquoise);

        foreach (var r in roles.Roles) {
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
    public async Task AssignRole(InteractionContext ctx) {
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        // Create role options and dropdowns
        var roleOptions = new List<DiscordSelectComponentOption>();
        foreach (var r in roles) {
            roleOptions.Add(new DiscordSelectComponentOption(r.Role.Name, r.Role.Id.ToString(), r.Description, false, new DiscordComponentEmoji(r.Emoji)));
        }
        var roleDropdown = new DiscordSelectComponent(IDHelper.Roles.ASSIGN, "Select a role", roleOptions);
        var cancelButton = new DiscordButtonComponent(ButtonStyle.Secondary, IDHelper.Roles.ASSIGN, "Cancel", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":arrow_left:", false)));
        
        var embed = new DiscordEmbedBuilder()
            .WithTitle(roles.CategoryName)
            .WithDescription(roles.CategoryDescription)
            .WithColor(DiscordColor.Turquoise);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(roleDropdown).AddComponents(cancelButton)));

        ctx.Client.ComponentInteractionCreated -= RoleHandler.HandleAssign;
        ctx.Client.ComponentInteractionCreated += RoleHandler.HandleAssign;

    }
}

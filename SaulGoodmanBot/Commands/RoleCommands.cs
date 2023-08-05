using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Handlers;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("role", "Manage self-assigning roles in server")]
public class RoleCommands : ApplicationCommandModule {
    [SlashCommand("setup", "Setup how roles are assigned in your server")]
    [SlashCommandPermissions(Permissions.Administrator)]
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
    [SlashCommandPermissions(Permissions.Administrator)]
    public async Task AddRole(InteractionContext ctx,
        [Option("role", "Role to add")] DiscordRole role,
        [Option("description", "Description to add to the role")] string? description=null,
        [Option("emoji", "Emoji to represent the role")] string? emoji=null) {
        
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        if (roles.IsNotSetup()) {
            // error: roles not setup in server
            // TODO: handle not setup error
        } else {
            if (emoji != null) {
                if (DiscordEmoji.TryFromName(ctx.Client, emoji, true, out DiscordEmoji validEmoji)) {
                    roles.Add(new RoleComponent(role, description, validEmoji));
                    await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);
                } else {
                    // error: emoji error
                    await ctx.CreateResponseAsync(StandardOutput.Error("Invalid emoji"), ephemeral:true);
                }
            } else {
                roles.Add(new RoleComponent(role, description, null));
                await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);
            }
        }
    }

    [SlashCommand("remove", "Remove a role from the list of self-assignable roles")]
    [SlashCommandPermissions(Permissions.Administrator)]
    public async Task RemoveRole(InteractionContext ctx) {
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        if (roles.IsNotSetup()) {
            // error: rles not setup in server
            // TODO: handle not setup error
        } else {
            var roleOptions = new List<DiscordSelectComponentOption>() {
                new DiscordSelectComponentOption("Cancel", "cancel", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":x:", false)))
            };
            foreach (var role in roles.Roles) {
                roleOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), "", false));
            }
            var roleDropdown = new DiscordSelectComponent("removeroledropdown", "Select a role", roleOptions);

            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("Remove Role")
                    .WithColor(DiscordColor.DarkRed))
                .AddComponents(roleDropdown);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            ctx.Client.ComponentInteractionCreated += RoleHandler.HandleRemoveRole;
        }
    }

    [SlashCommand("menu", "Creates a menu for assigning and unassigning roles")]
    public async Task RoleMenu(InteractionContext ctx) {
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        if (roles.IsNotSetup()) {
            // error
        } else {
            var rolesOptions = new List<DiscordSelectComponentOption>();
            foreach (var role in roles.Roles) {
                if (role.Emoji != null) {
                    rolesOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), role.Description ?? string.Empty, false, new DiscordComponentEmoji(role.Emoji)));
                } else {
                    rolesOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), role.Description ?? string.Empty, false));
                }
            }

            DiscordSelectComponent roleDropdown;
            if (roles.AllowMultipleRoles) {
                roleDropdown = new("rolemenudropdown", "Select roles", rolesOptions, false, 1, rolesOptions.Count - 1);
            } else {
                roleDropdown = new("rolemenudropdown", "Select a role", rolesOptions, false);
            }

            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle(roles.CategoryName)
                    .WithDescription(roles.CategoryDescription)
                    .WithColor(DiscordColor.Turquoise))
                .AddComponents(roleDropdown);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            // TODO: handler
        }
    }
}

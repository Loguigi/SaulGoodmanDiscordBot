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

        // ERROR: roles not setup
        if (roles.IsNotSetup()) 
            await ctx.CreateResponseAsync(StandardOutput.Error("Self-assignable roles not setup yet. Use /role setup"), ephemeral:true); 

        // Add role
        else {
            if (roles.AlreadyExists(role)) {
                // ERROR: role already added
                await ctx.CreateResponseAsync(StandardOutput.Error($"{role.Mention} already added to {roles.CategoryName}"), ephemeral:true);
            } else if (emoji != null) {

                // Standard unicode emoji
                if (DiscordEmoji.TryFromUnicode(ctx.Client, emoji, out DiscordEmoji normalEmoji)) {
                    roles.Add(new RoleComponent(role, description, normalEmoji));
                    await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);

                // Not standard unicode emoji; possible guild emoji
                } else if (emoji.Contains(':')) {
                    var tryEmoji = string.Join("", emoji.SkipWhile(x => x != ':').TakeWhile(x => !char.IsNumber(x)));

                    if (DiscordEmoji.TryFromName(ctx.Client, tryEmoji, true, out DiscordEmoji guildEmoji)) {
                        roles.Add(new RoleComponent(role, description, guildEmoji));
                        await ctx.CreateResponseAsync(StandardOutput.Success($"Added {role.Mention} to {roles.CategoryName}"), ephemeral:true);

                    // ERROR: emoji error
                    } else
                        await ctx.CreateResponseAsync(StandardOutput.Error("Invalid emoji"), ephemeral:true);

                // ERROR: emoji error
                } else 
                    await ctx.CreateResponseAsync(StandardOutput.Error("Invalid emoji"), ephemeral:true);
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

        // ERROR: not setup
        if (roles.IsNotSetup()) 
            await ctx.CreateResponseAsync(StandardOutput.Error("Self-assignable roles not setup yet. Use /role setup"), ephemeral:true);

        // ERROR: no roles added
        else if (roles.IsEmpty())
            await ctx.CreateResponseAsync(StandardOutput.Error("No roles added. Use /role add"), ephemeral:true);

        // Display menu
        else {
            // Create role options
            var roleOptions = new List<DiscordSelectComponentOption>();
            foreach (var role in roles.Roles) {
                if (role.Emoji != null) {
                    roleOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), role.Description ?? string.Empty, false, new DiscordComponentEmoji(role.Emoji)));
                } else {
                    roleOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), role.Description ?? string.Empty, false));
                }
            }

            // Create dropdown
            DiscordSelectComponent roleDropdown;
            if (roles.AllowMultipleRoles)
                roleDropdown = new("rolemenudropdown", "Select roles", roleOptions, false, 1, roleOptions.Count);
            else
                roleDropdown = new("rolemenudropdown", "Select a role", roleOptions, false);

            // Send prompt
            var prompt = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "https://youtu.be/dQw4w9WgXcQ", ctx.Guild.IconUrl)
                .WithTitle(roles.CategoryName)
                .WithDescription(roles.CategoryDescription)
                .WithFooter(roles.AllowMultipleRoles ? "Can have multiple" : "Can only have one")
                .WithColor(DiscordColor.Turquoise);
            foreach (var r in roles.Roles) {
                if (r == roles.Roles.First())
                    prompt.AddField("Available Roles", r.Role.Mention);
                else
                    prompt.Fields.Where(x => x.Name == "Available Roles").First().Value += $", {r.Role.Mention}";
            }
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(prompt).AddComponents(roleDropdown)));

            // Add handler
            ctx.Client.ComponentInteractionCreated -= RoleHandler.HandleMenu;
            ctx.Client.ComponentInteractionCreated += RoleHandler.HandleMenu;
        }
    }

    [SlashCommand("assign", "Assign or unassign a role")]
    public async Task AssignRole(InteractionContext ctx) {
        var roles = new ServerRoles(ctx.Guild, ctx.Client);

        // ERROR: not setup
        if (roles.IsNotSetup()) 
            await ctx.CreateResponseAsync(StandardOutput.Error("Self-assignable roles not setup yet. Use /role setup"), ephemeral:true);

        // ERROR: no roles added
        else if (roles.IsEmpty())
            await ctx.CreateResponseAsync(StandardOutput.Error("No roles added. Use /role add"), ephemeral:true);

        else {
            // Create role options and dropdowns
            var roleOptions = new List<DiscordSelectComponentOption>() {new DiscordSelectComponentOption("Cancel", "cancel", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":x:")))};
            foreach (var role in roles.Roles) {
                if (role.Emoji != null) {
                    roleOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), role.Description ?? string.Empty, false, new DiscordComponentEmoji(role.Emoji)));
                } else {
                    roleOptions.Add(new DiscordSelectComponentOption(role.Role.Name, role.Role.Id.ToString(), role.Description ?? string.Empty, false));
                }
            }
            var roleDropdown = new DiscordSelectComponent("roleassigndropdown", "Select a role", roleOptions);
        
            // Display prompt
            var prompt = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle(roles.CategoryName)
                    .WithDescription(roles.CategoryDescription)
                    .WithColor(DiscordColor.Turquoise))
                .AddComponents(roleDropdown);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(prompt));

            // Add handler
            // TODO make handler
        }

    }
}

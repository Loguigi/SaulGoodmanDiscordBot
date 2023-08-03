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
}

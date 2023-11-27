using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library.Helpers;
using SaulGoodmanBot.Library.Roles;

namespace SaulGoodmanBot.Handlers;

public static class RoleHandler {
    public static async Task HandleMenu(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Roles.MENU)) {
            await Task.CompletedTask;
            return;
        }

        var roles = new ServerRoles(e.Guild, s);
        var user = await e.Guild.GetMemberAsync(e.User.Id);
        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.User.GlobalName, "", e.User.AvatarUrl)
            .WithTitle(roles.CategoryName)
            .WithColor(DiscordColor.Green);

        if (roles.AllowMultipleRoles) {
            foreach (var roleid in e.Values) {
                var r = roles.GetRole(ulong.Parse(roleid));
                roles.Roles.Remove(r);

                // Add any role selected
                if (user.Roles.Contains(r.Role)) {
                    // User already has selected role; continue
                    continue;
                } else {
                    // User does not have selected role; grant role
                    await user.GrantRoleAsync(r.Role);
                    if (!embed.Fields.Where(x => x.Name == "Added Roles").Any())
                        embed.AddField("Added Roles", r.Role.Mention);
                    else
                        embed.Fields.Where(x => x.Name == "Added Roles").First().Value += $", {r.Role.Mention}";
                }
            }
            
            // Remove any roles not selected
            foreach (var savedRoles in roles.Roles) {
                if (user.Roles.Contains(savedRoles.Role)) {
                    await user.RevokeRoleAsync(savedRoles.Role);
                    if (!embed.Fields.Where(x => x.Name == "Removed Roles").Any())
                        embed.AddField("Removed Roles", savedRoles.Role.Mention);
                    else
                        embed.Fields.Where(x => x.Name == "Removed Roles").First().Value += $", {savedRoles.Role.Mention}";
                }
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
            return;
        }

        var role = roles.GetRole(ulong.Parse(e.Values.First()));

        // Remove any other role that isn't the selected role
        foreach (var savedRoles in roles.Roles) {
            if (user.Roles.Contains(savedRoles.Role)) {
                await user.RevokeRoleAsync(savedRoles.Role);
                if (!embed.Fields.Where(x => x.Name == "Removed Roles").Any())
                    embed.AddField("Removed Roles", savedRoles.Role.Mention);
                else
                    embed.Fields.Where(x => x.Name == "Removed Roles").First().Value += $", {savedRoles.Role.Mention}";
            }
        }
        
        // Add role
        await user.GrantRoleAsync(role.Role);
        embed.AddField("Added Role", role.Role.Mention);

        await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
    }

    public static async Task HandleAssign(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Roles.ASSIGN)) {
            await Task.CompletedTask;
            return;
        }

        var roles = new ServerRoles(e.Guild, s);
        var user = await e.Guild.GetMemberAsync(e.User.Id);
        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.User.GlobalName, "https://youtu.be/TQzBlxwoDpc", e.User.AvatarUrl)
            .WithTitle(roles.CategoryName)
            .WithColor(DiscordColor.Turquoise);

        if (e.Values.First() == "cancel") {
            embed.WithDescription("Cancelled");
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
            return;
        }

        var role = roles.GetRole(ulong.Parse(e.Values.First())).Role;

        if (user.Roles.Contains(role)) {
            await user.RevokeRoleAsync(role);
            embed.AddField("Removed Role", role.Mention);
        } else if (roles.AllowMultipleRoles) {
            await user.GrantRoleAsync(role);
            embed.AddField("Added Role", role.Mention);
        } else {
            // Remove any other role that isn't the selected role
            foreach (var savedRoles in roles.Roles) {
                if (user.Roles.Contains(savedRoles.Role)) {
                    await user.RevokeRoleAsync(savedRoles.Role);
                    embed.AddField("Removed Role", savedRoles.Role.Mention);
                }
            }

            // Add role
            await user.GrantRoleAsync(role);
            embed.AddField("Added Role", role.Mention);
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());

        s.ComponentInteractionCreated -= HandleAssign;
    }

    public static async Task HandleRemoveRole(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Roles.REMOVE)) {
            await Task.CompletedTask;
            return;
        }

        var roles = new ServerRoles(e.Guild, s);
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Remove Role")
            .WithColor(DiscordColor.DarkRed);

        if (e.Values.First() == "cancel") {
            embed.WithDescription("Cancelled");
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            return;
        }

        var roleid = ulong.Parse(e.Values.First());
        roles.Remove(roleid);
        embed.WithDescription($"{e.Guild.GetRole(roleid).Mention} has been removed from {roles.CategoryName}");

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));

        s.ComponentInteractionCreated -= HandleRemoveRole;
    }

    public static async Task HandleServerRemoveRole(DiscordClient s, GuildRoleDeleteEventArgs e) {
        var roles = new ServerRoles(e.Guild, s);
        if (roles.AlreadyExists(e.Role)) {
            roles.Remove(e.Role.Id);
        }

        await Task.CompletedTask;
    }
}

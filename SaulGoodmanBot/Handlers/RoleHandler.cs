using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class RoleHandler {
    public static async Task HandleMenu(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "rolemenudropdown") {
            var roles = new ServerRoles(e.Guild, s);
            var user = await e.Guild.GetMemberAsync(e.User.Id);
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(e.User.GlobalName, "", e.User.AvatarUrl)
                .WithTitle(roles.CategoryName)
                .WithColor(DiscordColor.Green);

            if (roles.AllowMultipleRoles) {
                foreach (var roleid in e.Values) {
                    var role = roles.GetRole(ulong.Parse(roleid));
                    roles.Roles.Remove(role);

                    // Add any role selected
                    if (user.Roles.Contains(role.Role)) {
                        // User already has selected role; continue
                        continue;
                    } else {
                        // User does not have selected role; grant role
                        await user.GrantRoleAsync(role.Role);
                        if (embed.Fields.Where(x => x.Name == "Added Roles").Count() == 0)
                            embed.AddField("Added Roles", role.Role.Mention);
                        else
                            embed.Fields.Where(x => x.Name == "Added Roles").First().Value += $", {role.Role.Mention}";
                    }
                }
                
                // Remove any roles not selected
                foreach (var savedRoles in roles.Roles) {
                    if (user.Roles.Contains(savedRoles.Role)) {
                        await user.RevokeRoleAsync(savedRoles.Role);
                        if (embed.Fields.Where(x => x.Name == "Removed Roles").Count() == 0)
                            embed.AddField("Removed Roles", savedRoles.Role.Mention);
                        else
                            embed.Fields.Where(x => x.Name == "Removed Roles").First().Value += $", {savedRoles.Role.Mention}";
                    }
                }
            } else {
                var role = roles.GetRole(ulong.Parse(e.Values.First()));

                // Remove any other role that isn't the selected role
                foreach (var savedRoles in roles.Roles) {
                    if (user.Roles.Contains(savedRoles.Role)) {
                        await user.RevokeRoleAsync(savedRoles.Role);
                        if (embed.Fields.Where(x => x.Name == "Removed Roles").Count() == 0)
                            embed.AddField("Removed Roles", savedRoles.Role.Mention);
                        else
                            embed.Fields.Where(x => x.Name == "Removed Roles").First().Value += $", {savedRoles.Role.Mention}";
                    }
                }
                
                // Add role
                await user.GrantRoleAsync(role.Role);
                embed.AddField("Added Role", role.Role.Mention);
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
        }
    }

    public static async Task HandleAssign(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "roleassigndropdown") {
            var roles = new ServerRoles(e.Guild, s);
            var user = await e.Guild.GetMemberAsync(e.User.Id);
            var embed = new DiscordEmbedBuilder()
                .WithTitle(roles.CategoryName)
                .WithColor(DiscordColor.Turquoise);

            if (e.Values.First() == "cancel")
                await e.Interaction.DeleteOriginalResponseAsync();
            else {
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
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());

            s.ComponentInteractionCreated -= HandleAssign;
        }
    }

    public static async Task HandleRemoveRole(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "removeroledropdown") {
            var roles = new ServerRoles(e.Guild, s);
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Remove Role")
                .WithColor(DiscordColor.DarkRed);

            if (e.Values.First() == "cancel") {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder()
                    .AddEmbed(embed.WithDescription("Cancelled"))));
            } else {
                var roleid = ulong.Parse(e.Values.First());
                roles.Remove(roleid);

                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder()
                    .AddEmbed(embed.WithDescription($"{e.Guild.GetRole(roleid).Mention} has been removed from {roles.CategoryName}"))));
            }

            s.ComponentInteractionCreated -= HandleRemoveRole;
        }
    }
}

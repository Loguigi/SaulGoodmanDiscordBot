using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class RoleHandler 
{
    public static async Task HandleMenu(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Roles.MENU)) 
        {
            await Task.CompletedTask;
            return;
        }

        try 
        {
            var roles = new ServerRoles(e.Guild);
            var user = await e.Guild.GetMemberAsync(e.User.Id);
            var selected = new List<ServerRoles.RoleComponent>();
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(e.User.GlobalName, "", e.User.AvatarUrl)
                .WithTitle(roles.CategoryName)
                .WithColor(DiscordColor.Green);

            if (roles.AllowMultipleRoles) 
            {
                // save selected roles
                foreach (var roleid in e.Values) 
                {
                    selected.Add(roles[e.Guild.GetRole(ulong.Parse(roleid))]!);
                }

                // add selected roles
                foreach (var select in selected) 
                {
                    if (!roles.HasRole(user, select))
                        await user.GrantRoleAsync(select.Role);
                }

                // remove roles not selected
                foreach (var r in roles.Roles) 
                {
                    if (roles.HasRole(user, r) && !selected.Contains(r)) 
                    {
                        await user.RevokeRoleAsync(r.Role);
                        if (embed.Fields.All(x => x.Name != "Removed Roles"))
                            embed.AddField("Removed Roles", r.Role.Mention);
                        else
                            embed.Fields.First(x => x.Name == "Removed Roles").Value += $", {r.Role.Mention}";
                    }
                }
            } 
            else 
            {
                var role = roles[e.Guild.GetRole(ulong.Parse(e.Values.First()))]!;
                if (roles.HasRole(user, role))
                    throw new Exception($"You already have the role {role.Role.Mention}");

                await user.GrantRoleAsync(role.Role);
                foreach (var r in roles.Roles) 
                {
                    if (r != role && roles.HasRole(user, role)) 
                    {
                        await user.RevokeRoleAsync(r.Role);
                        if (embed.Fields.All(x => x.Name != "Removed Roles"))
                            embed.AddField("Removed Roles", r.Role.Mention);
                        else
                            embed.Fields.First(x => x.Name == "Removed Roles").Value += $", {r.Role.Mention}";
                    }
                }
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public static async Task HandleAssign(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Roles.ASSIGN)) 
        {
            await Task.CompletedTask;
            return;
        }

        try 
        {
            var roles = new ServerRoles(e.Guild);
            var user = await e.Guild.GetMemberAsync(e.User.Id);
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(e.User.GlobalName, "https://youtu.be/TQzBlxwoDpc", e.User.AvatarUrl)
                .WithTitle(roles.CategoryName)
                .WithColor(DiscordColor.Turquoise);

            if (e.Values.First() == "cancel") 
            {
                embed.WithDescription("Cancelled");
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
                return;
            }

            var role = roles[e.Guild.GetRole(ulong.Parse(e.Values.First()))]!;

            if (roles.HasRole(user, role)) 
            {
                await user.RevokeRoleAsync(role.Role);
                embed.AddField("Removed Role", role.Role.Mention);
            } 
            else if (roles.AllowMultipleRoles) 
            {
                await user.GrantRoleAsync(role.Role);
                embed.AddField("Added Role", role.Role.Mention);
            } 
            else 
            {
                // Remove any other role that isn't the selected role
                foreach (var x in roles.Roles) 
                {
                    if (roles.HasRole(user, x)) 
                    {
                        await user.RevokeRoleAsync(x.Role);
                        embed.AddField("Removed Role", x.Role.Mention);
                    }
                }

                // Add role
                await user.GrantRoleAsync(role.Role);
                embed.AddField("Added Role", role.Role.Mention);
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());

        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        } 
        finally 
        {
            s.ComponentInteractionCreated -= HandleAssign;
        }
    }

    public static async Task HandleRemoveRole(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Roles.REMOVE)) 
        {
            await Task.CompletedTask;
            return;
        }

        try 
        {
            var roles = new ServerRoles(e.Guild);
            var role = roles[e.Guild.GetRole(ulong.Parse(e.Values.First()))]!;
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Remove Role")
                .WithColor(DiscordColor.DarkRed);

            if (e.Values.First() == "cancel") 
            {
                embed.WithDescription("Cancelled");
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
                return;
            }

            await roles.Remove(role);
            embed.WithDescription($"{role.Role.Mention} has been removed from {roles.CategoryName}");

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        } 
        finally 
        {
            s.ComponentInteractionCreated -= HandleRemoveRole;
        }
    }

    public static async Task HandleServerRemoveRole(DiscordClient s, GuildRoleDeleteEventArgs e) 
    {
        try 
        {
            var roles = new ServerRoles(e.Guild);
            if (roles[e.Role] != null) 
            {
                await roles.Remove(roles[e.Role]!);
            }

            await Task.CompletedTask;
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
}

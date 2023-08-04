using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class RoleHandler {
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

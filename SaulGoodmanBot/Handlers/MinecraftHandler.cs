using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class MinecraftHandler {
    public static async Task HandleDimensionChange(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (e.Id == "mcviewdropdown") {
            var minecraft = new Minecraft(e.Guild);
            var dimension = e.Values.First();

            // Create embed
            var embed = new DiscordEmbedBuilder()
                .WithAuthor("Minecraft", "", ImageHelper.Images["Minecraft"])
                .WithTitle(minecraft.WorldName)
                .WithDescription(minecraft.WorldDescription ?? string.Empty);
            
            if (minecraft.IPAddress != null) embed.AddField("IP Address", minecraft.IPAddress, true);
            if (minecraft.MaxPlayers != null) embed.AddField("Max Players", $"{minecraft.MaxPlayers}", true);
            embed.AddField("Whitelist", minecraft.Whitelist ? "Yes" : "No", true);

            // Add waypoints
            embed.AddField("Waypoints", "");
            if (minecraft.Waypoints.Where(x => x.Dimension == dimension).Count() == 0) {
                embed.Fields.Where(x => x.Name == "Waypoints").First().Value += $"No waypoints in {dimension}";
            } else {
                foreach (var waypoint in minecraft.Waypoints.Where(x => x.Dimension == dimension)) {
                    embed.Fields.Where(x => x.Name == "Waypoints").First().Value += $"*{waypoint.Name}* - `{waypoint.PrintCoords()}`";
                }
            }

            embed.WithColor(dimension switch {
                "overworld" => DiscordColor.Green,
                "nether" => DiscordColor.DarkRed,
                "end" => DiscordColor.Purple,
                _ => DiscordColor.Black
            });

            // Add dimension select dropdown
            var dimensionOptions = new List<DiscordSelectComponentOption>() {
                new DiscordSelectComponentOption("Overworld", "overworld", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":deciduous_tree:", false))),
                new DiscordSelectComponentOption("Nether", "nether", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":fire:", false))),
                new DiscordSelectComponentOption("The End", "end", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(s, ":milky_way:", false)))
            };
            var dimensionDropdown = new DiscordSelectComponent("mcviewdropdown", "Select a dimension", dimensionOptions);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dimensionDropdown)));
        }
    }
}

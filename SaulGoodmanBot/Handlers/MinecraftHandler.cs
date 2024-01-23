using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SaulGoodmanBot.Library.Minecraft;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class MinecraftHandler {
    public static async Task HandleDimensionChange(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        // TODO fix waypoint view interactivity
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
            if (minecraft.GetDimensionWaypoints(dimension).Count == 0) {
                embed.AddField("Waypoints", $"No waypoints in {dimension}");
            } else {
                foreach (var waypoint in minecraft.GetDimensionWaypoints(dimension)) {
                    if (embed.Fields.Where(x => x.Name == "Waypoints").FirstOrDefault() == null) {
                        embed.AddField("Waypoints", $"* *{waypoint.Name}* - `{waypoint.PrintCoords()}`\n");
                    } else {
                        embed.Fields.Where(x => x.Name == "Waypoints").First().Value += $"* *{waypoint.Name}* - `{waypoint.PrintCoords()}`\n";
                    }
                }
            }

            embed.WithColor(dimension switch {
                "overworld" => DiscordColor.SapGreen,
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

    public static async Task HandleWaypointDelete(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        // TODO fix delete interactivity
        if (e.Id == "wpdeletedropdown") {
            var minecraft = new Minecraft(e.Guild);
            var waypoint = minecraft.Waypoints.Where(x => x.Name == e.Values.First()).First();
            minecraft.DeleteWaypoint(waypoint);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Deleted waypoint")
                .AddField("Name", waypoint.Name, true)
                .AddField("Dimension", waypoint.Dimension, true)
                .AddField("Coords", waypoint.PrintCoords(), true)
                .WithColor(DiscordColor.DarkRed);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            s.ComponentInteractionCreated -= HandleWaypointDelete;

        } else if (e.Id == "wpdeletedropdown\\cancel") {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Cancelled")
                .WithColor(DiscordColor.DarkRed);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            s.ComponentInteractionCreated -= HandleWaypointDelete;
        }
    }

    public static async Task HandleWaypointList(DiscordClient s, ComponentInteractionCreateEventArgs e) {
        if (!e.Id.Contains(IDHelper.Minecraft.WAYPOINTLIST)) {
            await Task.CompletedTask;
            return;
        }

        var minecraft = new Minecraft(e.Guild);
        var dimension = e.Id.Split('\\')[DIMENSION_INDEX];
        var interactivity = new InteractivityHelper<Waypoint>(s, minecraft.GetDimensionWaypoints(dimension), $"{IDHelper.Minecraft.WAYPOINTLIST}\\{dimension}", e.Id.Split('\\')[PAGE_INDEX], 10, $"There are no waypoints in {dimension}");

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"{minecraft.WorldName} {dimension} waypoints")
            .WithDescription(interactivity.IsEmpty())
            .WithFooter(interactivity.PageStatus);
        embed.WithColor(dimension switch {
            "overworld" => DiscordColor.SapGreen,
            "nether" => DiscordColor.DarkRed,
            "end" => DiscordColor.Purple,
            _ => DiscordColor.Black
        });

        foreach (var w in interactivity) {
            embed.Description += $"* *{w.Name}* - `{w.PrintCoords()}`\n";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
    }

    private const int PAGE_INDEX = 2;
    private const int DIMENSION_INDEX = 1;
}

using DSharpPlus;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Handlers;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("minecraft", "Commands for working with Minecraft waypoints")]
public class MinecraftCommands : ApplicationCommandModule {
    [SlashCommand("save", "Saves information for a Minecraft server")]
    public async Task SaveInfo(InteractionContext ctx,
        [Option("name", "Name of the world/server")] string name,
        [Option("description", "Short description of the server")] string? description=null,
        [Option("ipaddress", "IP address of the server")] string? ip=null,
        [Option("maxplayers", "Maximum amount of players on a server")][Minimum(1)][Maximum(999999)] int? max=null,
        [Option("whitelist", "Specifies if the server has a whitelist")] bool whitelist=false) {
        
        var minecraft = new Minecraft(ctx.Guild) {
            WorldName = name,
            WorldDescription = description,
            IPAddress = ip,
            MaxPlayers = max,
            Whitelist = whitelist
        };
        minecraft.SaveServerInfo();

        // TODO success message
    }

    [SlashCommand("ip", "View server IP if set")]
    public async Task CheckIP(InteractionContext ctx) {
        var minecraft = new Minecraft(ctx.Guild);
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Minecraft", "", ImageHelper.Images["Minecraft"])
            .WithTitle(minecraft.WorldName)
            .WithDescription($"## {minecraft.IPAddress ?? "No IP address set"}")
            .WithColor(DiscordColor.SapGreen);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
    }

    [SlashCommand("view", "Creates view of all server info and waypoints")]
    public async Task View(InteractionContext ctx) {
        var minecraft = new Minecraft(ctx.Guild);

        // Create embed
        var embed = new DiscordEmbedBuilder()
            .WithAuthor("Minecraft", "", ImageHelper.Images["Minecraft"])
            .WithTitle(minecraft.WorldName)
            .WithDescription(minecraft.WorldDescription ?? string.Empty)
            .WithColor(DiscordColor.Green);
        
        if (minecraft.IPAddress != null) embed.AddField("IP Address", minecraft.IPAddress, true);
        if (minecraft.MaxPlayers != null) embed.AddField("Max Players", $"{minecraft.MaxPlayers}", true);
        embed.AddField("Whitelist", minecraft.Whitelist ? "Yes" : "No", true);

        // Add waypoints
        embed.AddField("Waypoints", "");
        if (minecraft.Waypoints.Count == 0) {
            embed.Fields.Where(x => x.Name == "Waypoints").First().Value += "No waypoints in overworld";
        } else {
            foreach (var waypoint in minecraft.Waypoints) {
               embed.Fields.Where(x => x.Name == "Waypoints").First().Value += $"*{waypoint.Name}* - `{waypoint.PrintCoords()}`";
            }
        }

        // Add dimension select dropdown
        var dimensionOptions = new List<DiscordSelectComponentOption>() {
            new DiscordSelectComponentOption("Overworld", "overworld", "", true, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":deciduous_tree:", false))),
            new DiscordSelectComponentOption("Nether", "nether", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":fire:", false))),
            new DiscordSelectComponentOption("The End", "end", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":milky_way:", false)))
        };
        var dimensionDropdown = new DiscordSelectComponent("mcviewdropdown", "Select a dimension", dimensionOptions);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dimensionDropdown)));

        // Add interaction
        ctx.Client.ComponentInteractionCreated -= MinecraftHandler.HandleDimensionChange;
        ctx.Client.ComponentInteractionCreated += MinecraftHandler.HandleDimensionChange;
    }

    [SlashCommandGroup("waypoint", "Commands for dealing with waypoints")]
    public class WaypointCommands : ApplicationCommandModule {
        [SlashCommand("add", "Add a new waypoint")]
        public async Task AddWaypoint(InteractionContext ctx,
            [Option("name", "Name of the waypoint")] string name,
            [Option("x", "X coordinate")] ulong x,
            [Option("y", "Y coordinate")] ulong y,
            [Option("z", "Z coordinate")] ulong z,
            [Choice("Overworld", "overworld")]
            [Choice("Nether", "nether")]
            [Choice("The End", "end")]
            [Option("dimension", "Dimension the waypoint is located in")] string dimension="overworld") {
            
            var minecraft = new Minecraft(ctx.Guild);
            var waypoint = new Minecraft.Waypoint(dimension, name, (int)x, (int)y, (int)z);
            minecraft.SaveNewWaypoint(waypoint);

            // TODO too many waypoints error

            var embed = new DiscordEmbedBuilder()
                .WithDescription($"### Waypoint Added")
                .AddField("Name", waypoint.Name, true)
                .AddField("Dimension", waypoint.Dimension, true)
                .AddField("Coords", waypoint.PrintCoords(), true)
                .WithColor(DiscordColor.Green);
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
        }

        [SlashCommand("delete", "Deletes an available waypoint")]
        public async Task DeleteWaypoint(InteractionContext ctx) {
            // TODO setup delete handler
        }

        [SlashCommand("list", "Lists all waypoints created")]
        public async Task ListWaypoints(InteractionContext ctx,
            [Option("dimension", "Sort waypoints by dimension")] string dimension="overworld") {
            // TODO list command
        }
    }
}

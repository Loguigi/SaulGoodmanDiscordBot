using DSharpPlus;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library.Minecraft;
using SaulGoodmanBot.Library.Helpers;
using SaulGoodmanBot.Handlers;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.Attributes;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("mc", "Commands for working with Minecraft waypoints")]
public class MinecraftCommands : ApplicationCommandModule {
    [SlashCommand("save", "Saves information for a Minecraft server")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task SaveInfo(InteractionContext ctx,
        [Option("name", "Name of the world/server")] string name,
        [Option("description", "Short description of the server")] string? description=null,
        [Option("ipaddress", "IP address of the server")] string? ip=null,
        [Option("maxplayers", "Maximum amount of players on a server")][Minimum(1)][Maximum(999999)] long? max=null,
        [Option("whitelist", "Specifies if the server has a whitelist")] bool whitelist=false) {
        
        var minecraft = new Minecraft(ctx.Guild) {
            WorldName = name,
            WorldDescription = description,
            IPAddress = ip,
            MaxPlayers = (int?)max,
            Whitelist = whitelist
        };
        minecraft.UpdateServerInfo();

        await ctx.CreateResponseAsync(StandardOutput.Success("Updated Minecraft server data"), ephemeral:true);
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
            .WithColor(DiscordColor.SapGreen);
        
        if (minecraft.IPAddress != null) embed.AddField("IP Address", minecraft.IPAddress, true);
        if (minecraft.MaxPlayers != null) embed.AddField("Max Players", $"{minecraft.MaxPlayers}", true);
        embed.AddField("Whitelist", minecraft.Whitelist ? "Yes" : "No", true);

        // Add waypoints
        if (minecraft.GetDimensionWaypoints("overworld").Count == 0) {
            embed.AddField("Waypoints", "No waypoints in overworld");
        } else {
            foreach (var waypoint in minecraft.Waypoints) {
                if (embed.Fields.Where(x => x.Name == "Waypoints").FirstOrDefault() == null) {
                    embed.AddField("Waypoints", $"* *{waypoint.Name}* - `{waypoint.PrintCoords()}`\n");
                } else {
                    embed.Fields.Where(x => x.Name == "Waypoints").First().Value += $"* *{waypoint.Name}* - `{waypoint.PrintCoords()}`\n";
                }
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

    [SlashCommand("add_waypoint", "Add a new waypoint")]
    public async Task AddWaypoint(InteractionContext ctx,
        [Option("name", "Name of the waypoint")] string name,
        [Option("x", "X coordinate")] long x,
        [Option("y", "Y coordinate")] long y,
        [Option("z", "Z coordinate")] long z,
        [Choice("Overworld", "overworld")]
        [Choice("Nether", "nether")]
        [Choice("The End", "end")]
        [Option("dimension", "Dimension the waypoint is located in")] string dimension="overworld") {
        
        var minecraft = new Minecraft(ctx.Guild);
        var waypoint = new Waypoint(dimension, name, (int)x, (int)y, (int)z);

        if (minecraft.WaypointsFull(dimension)) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"Too many waypoints in {dimension}"));
            return;
        }

        minecraft.SaveNewWaypoint(waypoint);

        var embed = new DiscordEmbedBuilder()
            .WithDescription($"### Waypoint Added")
            .AddField("Name", waypoint.Name, true)
            .AddField("Dimension", waypoint.Dimension, true)
            .AddField("Coords", waypoint.PrintCoords(), true)
            .WithColor(DiscordColor.Green);
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
    }

    [SlashCommand("delete_waypoint", "Deletes an available waypoint")]
    public async Task DeleteWaypoint(InteractionContext ctx,
        [Choice("Overworld", "overworld")]
        [Choice("Nether", "nether")]
        [Choice("The End", "end")]
        [Option("dimension", "Dimension the waypoint is located in")] string dimension="overworld") {

        var minecraft = new Minecraft(ctx.Guild);
        
        if (minecraft.GetDimensionWaypoints(dimension).Count == 0) {
            await ctx.CreateResponseAsync(StandardOutput.Error($"There are no waypoints in {dimension}"), ephemeral:true);
            return;
        }

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"Delete {dimension} waypoint")
            .WithColor(DiscordColor.DarkRed);

        var waypointOptions = new List<DiscordSelectComponentOption>();
        foreach (var waypoint in minecraft.GetDimensionWaypoints(dimension)) {
            waypointOptions.Add(new DiscordSelectComponentOption(waypoint.Name, waypoint.Name, waypoint.PrintCoords()));
        }
        var waypointDropdown = new DiscordSelectComponent("wpdeletedropdown", "Select a waypoint", waypointOptions, false);
        var cancelButton = new DiscordButtonComponent(ButtonStyle.Primary, "wpdeletedropdown\\cancel", "Cancel", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":x:", false)));

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(waypointDropdown).AddComponents(cancelButton)));

        ctx.Client.ComponentInteractionCreated -= MinecraftHandler.HandleWaypointDelete;
        ctx.Client.ComponentInteractionCreated += MinecraftHandler.HandleWaypointDelete;
    }
    
    [SlashCommand("list_waypoints", "Lists all waypoints created")]
    public async Task ListWaypoints(InteractionContext ctx,
        [Choice("Overworld", "overworld")]
        [Choice("Nether", "nether")]
        [Choice("The End", "end")]
        [Option("dimension", "Sort waypoints by dimension")] string dimension="overworld") {

        var minecraft = new Minecraft(ctx.Guild);
        var interactivity = new InteractivityHelper<Waypoint>(ctx.Client, minecraft.GetDimensionWaypoints(dimension), $"{IDHelper.Minecraft.WAYPOINTLIST}\\{dimension}", "1", $"There are no waypoints in {dimension}");

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"{minecraft.WorldName} {dimension} waypoints")
            .WithDescription(interactivity.IsEmpty())
            .WithFooter(interactivity.PageStatus());
        embed.WithColor(dimension switch {
            "overworld" => DiscordColor.SapGreen,
            "nether" => DiscordColor.DarkRed,
            "end" => DiscordColor.Purple,
            _ => DiscordColor.Black
        });

        foreach (var w in interactivity.GetPage()) {
            embed.Description += $"* *{w.Name}* - `{w.PrintCoords()}`\n";
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

        ctx.Client.ComponentInteractionCreated -= MinecraftHandler.HandleWaypointList;
        ctx.Client.ComponentInteractionCreated += MinecraftHandler.HandleWaypointList;
    }
}

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

public class LevelCommands : ApplicationCommandModule {
    [SlashCommand("level", "Check your own level or another user's level")]
    public async Task CheckLevel(InteractionContext ctx,
        [Option("user", "User to check the level of")] DiscordUser? user=null) {
        
        var level = new Levels(ctx.Guild, user ?? ctx.User, DateTime.Now);

        var message = new DiscordEmbedBuilder()
            .AddField("Rank", $"#{level.GetRank()}", true)
            .AddField("Level", level.Level.ToString(), true)
            .AddField("Experience", $"{level.Experience} / {level.ExpNeededForNextLevel()} XP", true)
            .WithThumbnail(level.User.AvatarUrl)
            .WithColor(DiscordColor.Violet);
        
        switch(level.GetRank()) {
            case 1: message.WithDescription($"## {DiscordEmoji.FromName(ctx.Client, ":first_place:")} {level.User.Mention}"); break;
            case 2: message.WithDescription($"## {DiscordEmoji.FromName(ctx.Client, ":second_place:")} {level.User.Mention}"); break;
            case 3: message.WithDescription($"## {DiscordEmoji.FromName(ctx.Client, ":third_place:")} {level.User.Mention}"); break;
            default: message.WithDescription($"## {level.User.Mention}"); break;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(message)));
    }
}

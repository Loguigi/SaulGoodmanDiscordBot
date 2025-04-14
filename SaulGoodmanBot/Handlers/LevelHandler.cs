using SaulGoodmanBot.Library;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using SaulGoodmanBot.Helpers;
using System.Reflection;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot.Handlers;

public static class LevelHandler 
{
    public static async Task HandleExpGain(DiscordClient s, MessageCreateEventArgs e) 
    {
        try
        {
            bool levelledUp = false;
            var config = new ServerConfig(e.Guild);
            if (e.Author.IsBot || !config.EnableLevels) 
            {
                await Task.CompletedTask;
                return;
            }
            var levels = new Levels(e.Guild);
            var user = levels[e.Author];

            //if (DateTime.Now >= user.MsgLastSent.AddMinutes(1)) 
            //{
                levelledUp = await levels.GrantExp(user);
                user = levels[e.Author];
            //}

            if (levelledUp) 
            {
                var embed = new DiscordEmbedBuilder()
                    .WithDescription($"### {DiscordEmoji.FromName(s, ":arrow_up:")} {e.Author.Mention} {config.LevelUpMessage}")
                    .WithFooter($"Level {user.Level - 1} {DiscordEmoji.FromName(s, ":arrow_right:")} Level {user.Level}", e.Author.AvatarUrl)
                    .WithColor(DiscordColor.Cyan);
                await config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent(e.Author.Mention).AddMention(new UserMention(e.Author)).AddEmbed(embed));
            }
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public static async Task HandleLeaderboard(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Levels.LEADERBOARD)) 
        {
            await Task.CompletedTask;
            return;
        }

        try 
        {
            var leaderboard = new Levels(e.Guild).GetInteractivity(IDHelper.GetId(e.Id, PAGE_INDEX));

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(e.Guild.Name, "https://youtu.be/nQGodNKogEI", e.Guild.IconUrl)
                .WithTitle("Server Leaderboard")
                .WithDescription("")
                .WithFooter(leaderboard.PageStatus)
                .WithColor(DiscordColor.Orange);
            
            foreach (var user in leaderboard) 
            {
                embed.Description += user.Rank switch 
                {
                    1 => $"### {DiscordEmoji.FromName(s, ":first_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    2 => $"### {DiscordEmoji.FromName(s, ":second_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    3 => $"### {DiscordEmoji.FromName(s, ":third_place:")} {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                    _ => $"### **__#{user.Rank}__** {user.User.Mention} `LVL {user.Level}` `EXP {user.Experience}`\n",
                };
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(leaderboard.AddPageButtons().AddEmbed(embed)));
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private const int PAGE_INDEX = 1;
}

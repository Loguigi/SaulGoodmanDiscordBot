using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class LevelHandlers(
    ServerMemberManager memberManager,
    ServerConfigManager configManager,
    ILogger<LevelHandlers> logger)
    : IEventHandler<MessageCreatedEventArgs>, IEventHandler<ComponentInteractionCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient s, MessageCreatedEventArgs e)
    {
        if (e.Author.IsBot) return;
        
        try
        {
            var member = await memberManager.GetMember(e.Author, e.Guild);
            var config = await configManager.GetConfig(e.Guild);
            var levelUp = await memberManager.GrantExp(member);

            if (levelUp)
            {
                logger.LogInformation("User {User} leveled up to level {Level} in guild {Guild}", 
                    e.Author.Username, member.Level, e.Guild.Name);
                
                var defaultChannel = config.DefaultChannel ?? e.Guild.GetDefaultChannel();
                await defaultChannel!.SendMessageAsync(MessageTemplates.CreateLevelUpNotification(member, config.LevelUpMessage));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message for level system from user {User} in guild {Guild}", 
                e.Author.Username, e.Guild.Name);
        }
    }

    public async Task HandleEventAsync(DiscordClient s, ComponentInteractionCreatedEventArgs e)
    {
        if (e.Id.Contains(IDHelper.Levels.LEADERBOARD))
        {
            logger.LogDebug("Leaderboard interaction from {User}", e.User.Username);
            await HandleLeaderboard(e);
        }
    }

    private async Task HandleLeaderboard(ComponentInteractionCreatedEventArgs e)
    {
        try
        {
            var page = IDHelper.GetId(e.Id, 1);
            var members = await memberManager.GetMembersAsync(e.Guild);

            await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateLeaderboard(members, e.Guild, page)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling leaderboard for guild {Guild}", e.Guild.Name);
            throw;
        }
    }
}
using DSharpPlus;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class MessageCreatedHandler(
    ServerMemberManager memberManager, 
    ServerConfigManager configManager, 
    ILogger<MessageCreatedHandler> logger) 
    : IEventHandler<MessageCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient s, MessageCreatedEventArgs e)
    {
        await HandleExperienceGain(e);
    }

    private async Task HandleExperienceGain(MessageCreatedEventArgs e)
    {
        if (e.Author.IsBot) return;
    
        try
        {
            var member = await memberManager.GetMember(e.Author, e.Guild);
            var config = await configManager.GetConfig(e.Guild);

            if (!config.EnableLevels) return;
            
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
}
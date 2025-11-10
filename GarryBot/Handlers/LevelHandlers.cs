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
    : ComponentInteractionHandler<LevelHandlers>(logger), 
        IEventHandler<MessageCreatedEventArgs>
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
                Logger.LogInformation("User {User} leveled up to level {Level} in guild {Guild}", 
                    e.Author.Username, member.Level, e.Guild.Name);
            
                var defaultChannel = config.DefaultChannel ?? e.Guild.GetDefaultChannel();
                await defaultChannel!.SendMessageAsync(MessageTemplates.CreateLevelUpNotification(member, config.LevelUpMessage));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing message for level system from user {User} in guild {Guild}", 
                e.Author.Username, e.Guild.Name);
        }
    }

    protected override Task RouteInteraction(string id, ComponentInteractionCreatedEventArgs e)
    {
        return id switch
        {
            IDHelper.Levels.LEADERBOARD => HandleLeaderboard(e),
            _ => Task.CompletedTask
        };
    }

    private async Task HandleLeaderboard(ComponentInteractionCreatedEventArgs e)
    {
        var members = await memberManager.GetMembersAsync(e.Guild);
        
        await HandlePagedResponse(e, 
            page => MessageTemplates.CreateLeaderboard(members, e.Guild, page),
            "leaderboard");
    }
}
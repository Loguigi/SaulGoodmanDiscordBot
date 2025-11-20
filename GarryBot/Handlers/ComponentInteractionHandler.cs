using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class ComponentInteractionHandler(
    ServerMemberManager memberManager,
    ServerConfigManager configManager,
    WheelPickerManager wheelPickerManager,
    ILogger<ComponentInteractionHandler> logger) 
    : BaseMessageSender, IEventHandler<ComponentInteractionCreatedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient s, ComponentInteractionCreatedEventArgs e)
    {
        var id = IDHelper.GetId(e.Id, 0);
        logger.LogDebug("Component interaction received: {ComponentId} from {User} in {Guild}", 
            id, e.User.Username, e.Guild.Name);
        
        await RouteInteraction(id, e);
    }

    private Task RouteInteraction(string id, ComponentInteractionCreatedEventArgs e)
    {
        return id switch
        {
            // Levels
            IDHelper.Levels.LEADERBOARD => HandleLeaderboard(e),
            
            // Wheel Picker
            IDHelper.WheelPicker.Spin => HandleWheelSpin(e),
            IDHelper.WheelPicker.List => HandleWheelList(e),
            IDHelper.WheelPicker.DeleteWheel => HandleDeleteWheel(e),
            IDHelper.WheelPicker.DeleteOption => HandleDeleteOption(e),
            
            // Secret Santa
            IDHelper.Santa.PARTICIPANTS => HandleSantaParticipants(e),
            IDHelper.Santa.GIFTSTATUSES => HandleGiftStatuses(e),
            
            // Misc
            IDHelper.Misc.EGG => HandleEggCounter(e),
            IDHelper.Misc.WHO => HandleIdentityList(e),
            
            _ => Task.CompletedTask
        };
    }

    #region Levels
    private async Task HandleLeaderboard(ComponentInteractionCreatedEventArgs e)
    {
        await HandlePagedResponse(e, 
            async page =>
            {
                var members = await memberManager.GetMembersAsync(e.Guild);
                return MessageTemplates.CreateLeaderboard(members, e.Guild, page);
            },
            "leaderboard");
    }
    #endregion

    #region Wheel Picker

    private async Task HandleWheelSpin(ComponentInteractionCreatedEventArgs e)
    {
        
    }
    
    private async Task HandleWheelList(ComponentInteractionCreatedEventArgs e)
    {
        await HandlePagedResponse(e,
            async page =>
            {
                var wheelPickers = await wheelPickerManager.GetAllAsync(e.Guild);
                throw new NotImplementedException();
            }, "wheel list");
    }

    private Task HandleDeleteWheel(ComponentInteractionCreatedEventArgs e)
    {
        // TODO: Implement wheel deletion
        return Task.CompletedTask;
    }

    private Task HandleDeleteOption(ComponentInteractionCreatedEventArgs e)
    {
        // TODO: Implement option deletion
        return Task.CompletedTask;
    }
    #endregion

    #region Secret Santa
    private Task HandleSantaParticipants(ComponentInteractionCreatedEventArgs e)
    {
        // TODO: Implement participants list
        return Task.CompletedTask;
    }

    private Task HandleGiftStatuses(ComponentInteractionCreatedEventArgs e)
    {
        // TODO: Implement gift statuses
        return Task.CompletedTask;
    }
    #endregion

    #region Misc
    private async Task HandleEggCounter(ComponentInteractionCreatedEventArgs e)
    {
        await HandlePagedResponse(e,
            async page =>
            {
                var members = await memberManager.GetMembersAsync(e.Guild);
                return MessageTemplates.CreateEggCounter(e.Guild, members, page);
            },
            "egg counter");
    }

    private async Task HandleIdentityList(ComponentInteractionCreatedEventArgs e)
    {
        await HandlePagedResponse(e,
            async page =>
            {
                var members = await memberManager.GetMembersAsync(e.Guild);
                return MessageTemplates.CreateIdentityDisplay(e.Guild, members, page);
            },
            "identity list");
    }
    #endregion

    #region Helper Methods
    private async Task HandlePagedResponse(
        ComponentInteractionCreatedEventArgs e,
        Func<string, Task<DiscordInteractionResponseBuilder>> messageBuilder,
        string handlerName)
    {
        try
        {
            logger.LogDebug("Handling {Handler} for user {User} in {Guild}", 
                handlerName, e.User.Username, e.Guild.Name);
            
            var page = IDHelper.GetId(e.Id, 1);
            var message = await messageBuilder(page);
            
            await e.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.UpdateMessage,
                message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {Handler} for user {User} in {Guild}", 
                handlerName, e.User.Username, e.Guild.Name);
            throw;
        }
    }
    #endregion
}
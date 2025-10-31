using DSharpPlus;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class MiscHandlers(
    ServerMemberManager memberManager,
    Random random,
    ILogger<MiscHandlers> logger)
    : IEventHandler<ComponentInteractionCreatedEventArgs>, IEventHandler<VoiceStateUpdatedEventArgs>
{
    public Task HandleEventAsync(DiscordClient s, ComponentInteractionCreatedEventArgs e)
    {
        var id = IDHelper.GetId(e.Id, 0);

        logger.LogDebug("Component interaction received: {ComponentId} from {User}", id, e.User.Username);

        return id switch
        {
            IDHelper.Misc.EGG => HandleEggCounter(e),
            _ => Task.CompletedTask
        };
    }

    public Task HandleEventAsync(DiscordClient s, VoiceStateUpdatedEventArgs e)
    {
        // Check if user left a voice channel
        if (e.Before?.Channel != null && e.After?.Channel != e.Before.Channel)
        {
            var channelLeftFrom = e.Before.Channel;
            
            if (channelLeftFrom.Users.Count() == 1)
            {
                var remainingMember = channelLeftFrom.Users.First();
                logger.LogInformation("{User} left voice channel {Channel}, {RemainingUser} is now alone", 
                    e.User.Username, channelLeftFrom.Name, remainingMember.Username);
                
                // Your logic here
            }
        }
        
        return Task.CompletedTask;
    }

    public async Task HandleEggCounter(ComponentInteractionCreatedEventArgs e)
    {
        try
        {
            logger.LogDebug("Handling egg counter for user {User}", e.User.Username);
            // Your logic here
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling egg counter for user {User}", e.User.Username);
            throw;
        }
    }
}
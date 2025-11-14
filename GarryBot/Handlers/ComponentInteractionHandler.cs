using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public abstract class ComponentInteractionHandler<THandler>(ILogger<THandler> logger) 
    : BaseMessageSender, IEventHandler<ComponentInteractionCreatedEventArgs>
{
    protected ILogger<THandler> Logger { get; } = logger;
    
    public Task HandleEventAsync(DiscordClient s, ComponentInteractionCreatedEventArgs e)
    {
        var id = IDHelper.GetId(e.Id, 0);
        Logger.LogDebug("Component interaction received: {ComponentId} from {User} in {Guild}", id, e.User.Username, e.Guild.Name);
        
        return RouteInteraction(id, e);
    }
    
    protected abstract Task RouteInteraction(string id, ComponentInteractionCreatedEventArgs e);
    
    protected async Task HandleWithLogging(
        ComponentInteractionCreatedEventArgs e, 
        Func<ComponentInteractionCreatedEventArgs, Task> handler,
        string handlerName)
    {
        try
        {
            Logger.LogDebug("Handling {Handler} for user {User} in {Guild}", handlerName, e.User.Username, e.Guild.Name);
            await handler(e);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling {Handler} for user {User} in {Guild}", handlerName, e.User.Username, e.Guild.Name);
            throw;
        }
    }
    
    protected async Task HandlePagedResponse(
        ComponentInteractionCreatedEventArgs e,
        Func<string, DiscordInteractionResponseBuilder> messageBuilder,
        string handlerName)
    {
        await HandleWithLogging(e, async args =>
        {
            var page = IDHelper.GetId(args.Id, 1);
            await args.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.UpdateMessage,
                messageBuilder(page));
        }, handlerName);
    }
}
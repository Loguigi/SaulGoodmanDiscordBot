using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

public abstract class BaseCommand<TCommands>(ILogger<TCommands> logger) : BaseMessageSender
{
    protected async Task ExecuteAsync(SlashCommandContext ctx, Func<Task> action, string commandName)
    {
        try
        {
            logger.LogInformation("{Command} command invoked by {User} in guild {Guild}", 
                commandName, ctx.User.Username, ctx.Guild?.Name);
            
            await action();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing {Command} command for user {User}", 
                commandName, ctx.User.Username);
            await SendMessage(ctx, MessageTemplates.CreateError("Error executing command"), true);
        }
    }
}
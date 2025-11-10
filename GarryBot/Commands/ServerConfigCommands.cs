using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

public class ServerConfigCommands(
    ServerConfigManager configManager,
    ILogger<ServerConfigCommands> logger)
    : BaseCommand<ServerConfigCommands>(logger)
{
    
}
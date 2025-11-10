using DSharpPlus;
using DSharpPlus.Commands;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

[Command("wheel")]
public class WheelPickerCommands(
    WheelPickerManager wheelManager,
    ILogger<WheelPickerCommands> logger)
    : BaseCommand<WheelPickerCommands>(logger)
{
    
}
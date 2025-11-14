using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

[Command("wheel")]
public class WheelPickerCommands(
    WheelPickerManager wheelManager,
    ServerMemberManager memberManager,
    Random random,
    ILogger<WheelPickerCommands> logger)
    : BaseCommand<WheelPickerCommands>(logger)
{
    private readonly ILogger<WheelPickerCommands> _logger = logger;

    [Command("create")]
    public async Task CreateWheel(SlashCommandContext ctx,
        string name,
        DiscordAttachment? image)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var currentWheels = await wheelManager.GetAllAsync(ctx.Guild!);
            
            if (currentWheels.Any(x => x.Name == name))
            {
                await SendMessage(ctx, MessageTemplates.CreateError("A wheel with that name already exists"), true);
                return;
            }

            await wheelManager.CreateWheelAsync(new WheelPicker()
            {
                GuildId = (long)ctx.Guild!.Id,
                Name = name,
                ImageUrl = image?.Url,
            });
            
            await SendMessage(ctx, MessageTemplates.CreateSuccess($"Wheel created with name {name}"), true);
        }, "create");
    }
    
    [Command("add")]
    public async Task AddOption(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName,
        string? option = null,
        DiscordAttachment? file = null)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var (hasWheels, wheels) = await Validation.GetWheelsOrError(ctx, wheelManager);
            if (!hasWheels) return;
            
            var wheel = wheels.FirstOrDefault(w => w.Name == wheelName);
            if (wheel == null)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Wheel not found"), true);
                return;
            }

            if (file != null)
            {
                // Validate file type
                if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    await SendMessage(ctx, MessageTemplates.CreateError("Please upload a .txt file"), true);
                    return;
                }

                try
                {
                    // Download the file content
                    using var httpClient = new HttpClient();
                    var fileContent = await httpClient.GetStringAsync(file.Url);
        
                    // Parse lines and filter out empty ones
                    var options = fileContent
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToList();

                    if (options.Count == 0)
                    {
                        await SendMessage(ctx, MessageTemplates.CreateError("No valid options found in file"), true);
                        return;
                    }

                    // Add each option to the wheel
                    foreach (var opt in options)
                    {
                        await wheelManager.AddOptionAsync(wheel, opt);
                    }

                    await SendMessage(ctx,
                        MessageTemplates.CreateSuccess($"Added {options.Count} option(s) to wheel '{wheelName}'"), 
                        true);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Failed to download file from Discord");
                    await SendMessage(ctx, MessageTemplates.CreateError("Failed to download file"), true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file");
                    await SendMessage(ctx, MessageTemplates.CreateError("Error processing file"), true);
                }
            }
            else if (option != null)
            {
                await wheelManager.AddOptionAsync(wheel, option);
                await SendMessage(ctx, MessageTemplates.CreateSuccess($"Added option '{option}' to wheel '{wheelName}'"), true);
            }
        }, "add");
    }

    [Command("spin")]
    public async Task Spin(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var (hasWheels, wheels) = await Validation.GetWheelsOrError(ctx, wheelManager);
            if (!hasWheels) return;
            
            var wheel = wheels.FirstOrDefault(w => w.Name == wheelName);
            if (wheel == null)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Wheel not found"), true);
                return;
            }

            if (wheel.AvailableOptions.Count == 0)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("No options available in this wheel"), true);
                return;
            }

            var member = await memberManager.GetMember(ctx.User, ctx.Guild!);
            var result = wheel.Spin(random);

            // First spin: no previous option
            var spinData = new SpinData(wheelName, 1, result!.Option, null, false);
        
            await ctx.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(
                    MessageTemplates.CreateWheelSpin(member, wheel, spinData)));
        }, "spin");
    }

    [Command("delete")]
    public async Task DeleteWheel(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var (hasWheels, wheels) = await Validation.GetWheelsOrError(ctx, wheelManager);
            if (!hasWheels) return;
            
            var wheel = wheels.FirstOrDefault(w => w.Name == wheelName);
            if (wheel == null)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Wheel not found"), true);
                return;
            }
        }, "delete");
    }

    [Command("reload")]
    public async Task ReloadWheel(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var (hasWheels, wheels) = await Validation.GetWheelsOrError(ctx, wheelManager);
            if (!hasWheels) return;
            
            var wheel = wheels.FirstOrDefault(w => w.Name == wheelName);
            if (wheel == null)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Wheel not found"), true);
                return;
            }
        }, "reload");
    }

    [Command("list")]
    public async Task ListWheelOptions(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var (hasWheels, wheels) = await Validation.GetWheelsOrError(ctx, wheelManager);
            if (!hasWheels) return;
            
            var wheel = wheels.FirstOrDefault(w => w.Name == wheelName);
            if (wheel == null)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Wheel not found"), true);
                return;
            }
        }, "list");
    }
}

public class WheelAutoCompleteProvider : IAutoCompleteProvider
{
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
    {
        var wheelManager = context.ServiceProvider.GetRequiredService<WheelPickerManager>();
        var wheels = await wheelManager.GetAllAsync(context.Guild!);

        var input = context.UserInput?.ToLower() ?? string.Empty;

        return wheels
            .Where(w => w.Name.ToLower().Contains(input))
            .Take(25)
            .Select(w => new DiscordAutoCompleteChoice(w.Name, w.Name));
    }
}
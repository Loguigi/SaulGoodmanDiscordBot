using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarryBot.Commands;

[Command("wheel"), Description("Manage and spin custom wheel pickers"), RequireGuild]
public class WheelPickerCommands(
    WheelPickerManager wheelManager,
    ServerMemberManager memberManager,
    Random random,
    ILogger<WheelPickerCommands> logger)
    : BaseCommand<WheelPickerCommands>(logger)
{
    private readonly ILogger<WheelPickerCommands> _logger = logger;

    [Command("create"), Description("Create a new wheel picker"), RequireGuild]
    public async Task CreateWheel(SlashCommandContext ctx,
        string name,
        DiscordAttachment? image=null)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var currentWheels = await wheelManager.GetAllAsync(ctx.Guild!);
            
            if (currentWheels.Count == 25)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Maximum number of wheels reached in the server. Please delete one to add another"), true);
                return;
            }
            
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
        }, "wheel create");
    }
    
    [Command("add"), Description("Add options to a wheel picker, either from a text file or manually"), RequireGuild]
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
                if (!file.FileName!.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
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
                        .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToList();

                    if (options.Count == 0)
                    {
                        await SendMessage(ctx, MessageTemplates.CreateError("No valid options found in file"), true);
                        return;
                    }

                    int counter = 0;
                    // Add each option to the wheel
                    foreach (var opt in options)
                    {
                        if (wheel.WheelOptions.Any(o => string.Equals(o.Option.Trim(), opt.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                            continue;

                        counter++;
                        await wheelManager.AddOptionAsync(wheel, opt);
                    }

                    await SendMessage(ctx,
                        MessageTemplates.CreateSuccess($"Added {counter} option(s) to wheel '{wheelName}'"), 
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
                if (wheel.WheelOptions.Any(o => string.Equals(o.Option.Trim(), option.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    await SendMessage(ctx, MessageTemplates.CreateError("Option already exists"), true);
                    return;
                }
                
                await wheelManager.AddOptionAsync(wheel, option);
                await SendMessage(ctx, MessageTemplates.CreateSuccess($"Added option '{option}' to wheel '{wheelName}'"), true);
            }
        }, "wheel add");
    }

    [Command("spin"), Description("Spin a wheel picker and get the result"), RequireGuild]
    public async Task Spin(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var wheel = await wheelManager.GetWheelByName(wheelName, ctx.Guild!);
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
            var spinData = new SpinData(wheel.Id, 1, result!.Option, null, false);

            await SendMessage(ctx, MessageTemplates.CreateWheelSpin(member, wheel, spinData));
        }, "wheel spin");
    }

    [Command("delete"), Description("Delete a wheel picker and all its options"), RequireGuild]
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
            
            await wheelManager.DeleteWheelAsync(wheel);
            await SendMessage(ctx, MessageTemplates.CreateSuccess($"Wheel '{wheelName}' deleted"));
        }, "wheel delete");
    }

    [Command("remove"), Description("Remove an option from a wheel picker"), RequireGuild]
    public async Task RemoveOption(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<WheelAutoCompleteProvider>] string wheelName,
        [Description("Id of option to remove")] long optionId)
    {
        await ExecuteAsync(ctx, async () =>
        {
            var wheel = await wheelManager.GetWheelByName(wheelName, ctx.Guild!);
            if (wheel == null)
            {
                await SendMessage(ctx, MessageTemplates.CreateError("Wheel not found"), true);
                return;
            }
            
            if (optionId < 1 && optionId > wheel.WheelOptions.Count)
            {
                await SendMessage(ctx, MessageTemplates.CreateError($"Invalid option id. Please select an id between 1 and {wheel.WheelOptions.Count}"), true);
                return;
            }

            var optionToDelete = wheel.WheelOptions[(int)optionId - 1];
            await wheelManager.RemoveOptionAsync(wheel, optionToDelete);
            await SendMessage(ctx, MessageTemplates.CreateSuccess($"Option `{optionToDelete.Option}` removed from wheel `{wheelName}`"));
        }, "wheel remove");
    }

    [Command("reload"), Description("Restore removed options for a wheel picker"), RequireGuild]
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

            var removedOptions = wheel.TempRemovedOptions.Count;
            await wheelManager.RestoreWheelOptions(wheel);

            await SendMessage(ctx, MessageTemplates.CreateSuccess($"Restored `{removedOptions}` options for {wheel.Name}"));
        }, "wheel reload");
    }

    [Command("list"), Description("List all options for a wheel picker"), RequireGuild]
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

            await SendMessage(ctx, MessageTemplates.CreateWheelOptionList(wheel, ctx.Guild!, "1"));
        }, "wheel list");
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
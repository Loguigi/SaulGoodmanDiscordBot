using DSharpPlus.Entities;
using GarryLibrary.Data;
using GarryLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GarryLibrary.Managers;

public class WheelPickerManager(
    IDataRepository<WheelPicker> wheelPickerRepository,
    IDataRepository<WheelOption> wheelOptionRepository)
{
    public async Task<List<WheelPicker>> GetAllAsync(DiscordGuild guild)
    {
        var wheels = await wheelPickerRepository.GetAllAsync(q =>
            q.Where(wp => wp.GuildId == (long)guild.Id)
                .Include(wp => wp.WheelOptions));

        foreach (var wheel in wheels)
        {
            OrderWheelOptions(wheel);
        }
        
        return wheels;
    }

    public async Task<WheelPicker?> GetWheelById(int id)
    {
        var wheel = (await wheelPickerRepository.GetAllAsync(q =>
            q.Where(wp => wp.Id == id)
                .Include(wp => wp.WheelOptions)))
        .FirstOrDefault();
        
        if (wheel != null) OrderWheelOptions(wheel);
        
        return wheel;
    }


    public async Task<WheelPicker?> GetWheelByName(string name, DiscordGuild guild)
    {
        var wheel = (await wheelPickerRepository.GetAllAsync(q =>
            q.Where(wp => wp.GuildId == (long)guild.Id && wp.Name == name)
                .Include(wp => wp.WheelOptions)))
        .FirstOrDefault();
        
        if (wheel != null) OrderWheelOptions(wheel);

        return wheel;
    }

    public async Task CreateWheelAsync(WheelPicker wheel) => await wheelPickerRepository.CreateAsync(wheel);
    
    public async Task UpdateWheelAsync(WheelPicker wheel) => await wheelPickerRepository.UpdateAsync(wheel);
    
    public async Task DeleteWheelAsync(WheelPicker wheel) => await wheelPickerRepository.DeleteAsync(wheel);
    
    public async Task AddOptionAsync(WheelPicker wheel, string option)
    {
        var newOption = new WheelOption()
        {
            WheelId = wheel.Id,
            WheelPicker = wheel,
            Option = option,
            TempRemoved = false,
        };
        
        await wheelOptionRepository.CreateAsync(newOption);
        wheel.WheelOptions.Add(newOption);
    }
    
    public async Task RemoveOptionAsync(WheelPicker wheel, WheelOption option)
    {
        await wheelOptionRepository.DeleteAsync(option);
        wheel.WheelOptions.Remove(option);
    }
    
    public async Task TempRemoveOptionAsync(WheelPicker wheel, WheelOption option)
    {
        option.TempRemoved = true;
        await wheelOptionRepository.UpdateAsync(option);
    }

    public async Task RestoreWheelOptions(WheelPicker wheel)
    {
        foreach (var option in wheel.TempRemovedOptions)
        {
            option.TempRemoved = false;
            await wheelOptionRepository.UpdateAsync(option);
        }
    }
    
    private void OrderWheelOptions(WheelPicker wheel) => wheel.WheelOptions = wheel.WheelOptions.OrderBy(wo => wo.TempRemoved).ToList();
}
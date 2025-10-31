using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace GarryBot.Services;

public class DailyEventService : IHostedService, IDisposable
{
    private readonly DiscordClient _discordClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyEventService> _logger;
    private Timer? _timer;

    public DailyEventService(
        DiscordClient discordClient,
        IServiceProvider serviceProvider,
        ILogger<DailyEventService> logger)
    {
        _discordClient = discordClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Daily Event Service starting...");
        
        // Calculate time until next midnight
        var now = DateTime.Now;
        var nextMidnight = DateTime.Today.AddDays(1);
        var timeUntilMidnight = nextMidnight - now;

        // Start timer that triggers at midnight, then every 24 hours
        _timer = new Timer(timeUntilMidnight.TotalMilliseconds);
        _timer.Elapsed += async (sender, e) => await OnTimerElapsed();
        _timer.AutoReset = false; // We'll reset it manually
        _timer.Start();

        _logger.LogInformation("Daily Event Service will first run at {NextRun}", nextMidnight);
        
        return Task.CompletedTask;
    }

    private async Task OnTimerElapsed()
    {
        try
        {
            _logger.LogInformation("Running daily event checks...");
            
            // Create a scope to resolve scoped services
            using var scope = _serviceProvider.CreateScope();
            
            // Resolve your managers
            var memberManager = scope.ServiceProvider.GetRequiredService<ServerMemberManager>();
            var configManager = scope.ServiceProvider.GetRequiredService<ServerConfigManager>();
            
            // Check all guilds the bot is in
            foreach (var guild in _discordClient.Guilds.Values)
            {
                await CheckGuildEvents(guild, memberManager, configManager);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during daily event check");
        }
        finally
        {
            // Reset timer for next 24 hours
            if (_timer != null) _timer.Interval = TimeSpan.FromHours(24).TotalMilliseconds;

            _timer?.Start();
        }
    }

    private async Task CheckGuildEvents(
        DiscordGuild guild, 
        ServerMemberManager memberManager, 
        ServerConfigManager configManager)
    {
        try
        {
            _logger.LogInformation("Checking events for guild: {GuildName}", guild.Name);
            
            var config = await configManager.GetConfig(guild);
            var members = await memberManager.GetMembersAsync(guild);
            
            // Check for birthdays
            await CheckBirthdays(guild, members, config);
            
            // Check for other events
            // await CheckSantaExchange(guild, config);
            // await CheckOtherEvents(guild);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking events for guild {GuildId}", guild.Id);
        }
    }

    private async Task CheckBirthdays(
        DiscordGuild guild, 
        List<ServerMember> members, 
        ServerConfig config)
    {
        var todaysBirthdays = members.Where(m => m.HasBirthdayToday == true).ToList();
        
        if (todaysBirthdays.Any())
        {
            _logger.LogInformation("Found {Count} birthday(s) in {GuildName}", 
                todaysBirthdays.Count, guild.Name);
            
            var channel = config.DefaultChannel ?? guild.GetDefaultChannel();
            
            foreach (var member in todaysBirthdays)
            {
                var embed = new GarryMessageBuilder()
                    .WithTitle("🎉 Happy Birthday! 🎉")
                    .WithDescription($"Happy birthday {member.User.Mention}! 🎂\nThey're turning **{member.Age}** today!")
                    .WithThumbnail(member.User.AvatarUrl)
                    .WithTheme(EmbedTheme.Birthday)
                    .WithTimestamp()
                    .ToEmbed();
                
                await channel!.SendMessageAsync(embed);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Daily Event Service stopping...");
        _timer?.Stop();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
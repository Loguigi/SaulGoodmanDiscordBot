using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Extensions;
using GarryBot.Commands;
using GarryBot.Handlers;
using GarryBot.Services;
using GarryLibrary.Data;
using GarryLibrary.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

string discordToken = Environment.GetEnvironmentVariable("GarryToken") ?? throw new Exception("Token not set");
var sqlCnn = Environment.GetEnvironmentVariable("GarryConnectionString");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
#if DEBUG
            logging.SetMinimumLevel(LogLevel.Information);
#else
            logging.SetMinimumLevel(LogLevel.Warning);
#endif
        });
    
        #region Data Source Service
        if (!string.IsNullOrEmpty(sqlCnn))
        {
            services.AddDbContext<GarryDbContext>(options =>
                options.UseSqlServer(sqlCnn));
        }
        else
        {
            services.AddDbContext<GarryDbContext>(options => options.UseInMemoryDatabase("Garry"));
        }

        services.AddScoped(typeof(IDataRepository<>), typeof(GarryRepository<>));
        #endregion
        
        #region Data Managers
        services.AddScoped<ServerMemberManager>();
        services.AddScoped<SecretSantaManager>();
        services.AddScoped<WheelPickerManager>();
        services.AddScoped<ServerConfigManager>();
        #endregion
        
        #region Other Services
        services.AddSingleton<Random>();
        services.AddHostedService<DailyEventService>();
        #endregion
    
        #region Discord Service
        services.AddDiscordClient(discordToken, DiscordIntents.All)
            .ConfigureEventHandlers(b =>
            {
                b.AddEventHandlers<ComponentInteractionHandler>(ServiceLifetime.Scoped);
                b.AddEventHandlers<MessageCreatedHandler>(ServiceLifetime.Scoped);
                b.AddEventHandlers<ScheduledGuildEventHandler>(ServiceLifetime.Scoped);
                b.AddEventHandlers<VoiceStateUpdatedEventHandler>(ServiceLifetime.Scoped);
            });

        services.AddCommandsExtension((provider, extension) =>
        {
            extension.AddCommands(typeof(Program).Assembly);
            
            TextCommandProcessor textCommandProcessor = new(new TextCommandConfiguration
            {
                PrefixResolver = new DefaultPrefixResolver(true, "`").ResolvePrefixAsync,
            });
            var slashCommandProcessor = new SlashCommandProcessor();
            extension.AddProcessor(textCommandProcessor);
            extension.AddProcessor(slashCommandProcessor);
        }, new CommandsConfiguration()
        {
            RegisterDefaultCommandProcessors = true,
        });
        #endregion
    })
.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting GarryBot");
    var client = host.Services.GetRequiredService<DiscordClient>();
    await client.ConnectAsync();
    logger.LogInformation("Connected to Discord");
    
    await host.RunAsync();
    await Task.Delay(-1);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Fatal error while connecting");
}
finally
{
    logger.LogInformation("Stopping GarryBot");
}
  
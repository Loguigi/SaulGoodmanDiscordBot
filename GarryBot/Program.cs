using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Extensions;
using GarryBot.Commands;
using GarryBot.Handlers;
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
            logging.SetMinimumLevel(LogLevel.Information);
        });
    
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
        services.AddScoped<ServerMemberManager>();
        services.AddScoped<SecretSantaManager>();
        services.AddScoped<WheelPickerManager>();
        services.AddScoped<ServerConfigManager>();
        
        services.AddScoped<LevelHandlers>();
        services.AddScoped<MiscHandlers>();
    
        services.AddDiscordClient(discordToken, DiscordIntents.All);
        services.AddCommandsExtension((provider, extension) =>
        {
            extension.AddCommands(typeof(BirthdayCommands).Assembly);
            extension.AddCommands(typeof(LevelCommands).Assembly);
            extension.AddCommands(typeof(MiscCommands).Assembly);
            extension.AddCommands(typeof(WheelPickerCommands).Assembly);
            
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
        services.AddSingleton<Random>();
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
  
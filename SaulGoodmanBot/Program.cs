using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace SaulGoodmanBot;

class Program {
    static async Task Main() {
        var builder = new HostBuilder()
            .ConfigureAppConfiguration(x => {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", false, true)
                    .Build();

                x.AddConfiguration(config);
            })
            .ConfigureLogging(x => {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureDiscordHost((context, config) => {
                config.SocketConfig = new DiscordSocketConfig {
                    LogLevel = Discord.LogSeverity.Debug,
                    AlwaysDownloadUsers = false,
                    MessageCacheSize = 200,
                };

                config.Token = context.Configuration["Token"];
            })
            .UseCommandService((context, config) => {
                config.CaseSensitiveCommands = false;
                config.LogLevel = Discord.LogSeverity.Debug;
                config.DefaultRunMode = RunMode.Sync;
            })
            .ConfigureServices((context, services) => {
                // services
                // .AddHostedService<CommandHandler>();
            })
            .UseConsoleLifetime();
        
        var host = builder.Build();
        using (host) {
            await host.RunAsync();
        }
    }
}
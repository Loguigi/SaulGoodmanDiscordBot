using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using SaulGoodmanBot.Config;
using SaulGoodmanBot.Commands;
using Microsoft.Extensions.Logging;

namespace SaulGoodmanBot.Source;

public class Bot {
    public DiscordClient? Client { get; private set; }
    public InteractivityExtension? Interactivity { get; private set; }
    public CommandsNextExtension? Commands { get; private set; }

    public async Task RunAsync() {
        var json = string.Empty;
        using (var fs = File.OpenRead("Config/config.json"))
        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync();

        var configJSON = JsonConvert.DeserializeObject<ConfigJSON>(json);

        // Config for Discord client
        var discordConfig = new DiscordConfiguration() {
            Intents = DiscordIntents.All,
            Token = configJSON.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
            MinimumLogLevel = LogLevel.Debug
        };

        Client = new DiscordClient(discordConfig);
        Client.UseInteractivity(new InteractivityConfiguration() {
            Timeout = TimeSpan.FromMinutes(2)
        });

        // Config for Discord commands
        var commandsConfig = new CommandsNextConfiguration() {
            StringPrefixes = new string[] { configJSON.Prefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
        };

        // Commands registration
        Commands = Client.UseCommandsNext(commandsConfig);

        // Slash commands registration
        var slashCommandsConfig = Client.UseSlashCommands();
        slashCommandsConfig.RegisterCommands<MiscCommands>();
        slashCommandsConfig.RegisterCommands<WheelPickerCommands>();

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }
    public static void Main() => new Bot().RunAsync().GetAwaiter().GetResult();
}
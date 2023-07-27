using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using SaulGoodmanBot.Config;
using SaulGoodmanBot.Commands;
using SaulGoodmanBot.Library;
using Microsoft.Extensions.Logging;
using DSharpPlus.EventArgs;

namespace SaulGoodmanBot;

public class Bot {
    // Discord Client Properties
    public static DiscordClient? Client { get; private set; }
    public static InteractivityExtension? Interactivity { get; private set; }
    public static CommandsNextExtension? Commands { get; private set; }

    public async Task MainAsync() {
        // Json Config Reader
        var json = string.Empty;
        using (var fs = File.OpenRead("Config/config.json"))
        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync();

        var configJSON = JsonConvert.DeserializeObject<ConfigJSON>(json);

        // Discord Client Config
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

        // Event Handlers
        Client.MessageCreated += BirthdayMessageHandler;

        // Commands Config
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
        slashCommandsConfig.RegisterCommands<ReactionCommands>();
        slashCommandsConfig.RegisterCommands<BirthdayCommands>();

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    public async Task BirthdayMessageHandler(DiscordClient s, MessageCreateEventArgs e) {
        var bdayList = new Birthdays(e.Guild.Id, s);
        await bdayList.CheckBirthdayToday();
    }

    public static void Main() => new Bot().MainAsync().GetAwaiter().GetResult();
}

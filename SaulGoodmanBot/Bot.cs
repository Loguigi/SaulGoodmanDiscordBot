using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using SaulGoodmanBot.Commands;

namespace SaulGoodmanBot;

public class Bot {
    public DiscordClient? Client { get; private set; }
    public InteractivityExtension? Interactivity { get; private set; }
    public CommandsNextExtension? Commands { get; private set; }

    public async Task RunAsync() {
        var json = string.Empty;
        using (var fs = File.OpenRead("config.json"))
        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync();

        var configJSON = JsonConvert.DeserializeObject<ConfigJSON>(json);

        var config = new DiscordConfiguration() {
            Intents = DiscordIntents.All,
            Token = configJSON.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };

        Client = new DiscordClient(config);
        Client.UseInteractivity(new InteractivityConfiguration() {
            Timeout = TimeSpan.FromMinutes(2)
        });

        var commandsConfig = new CommandsNextConfiguration() {
            StringPrefixes = new string[] { configJSON.Prefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
        };

        Commands = Client.UseCommandsNext(commandsConfig);

        Commands.RegisterCommands<TextCommands>();

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }
    public static void Main() => new Bot().RunAsync().GetAwaiter().GetResult();
}
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using SaulGoodmanBot.Config;
using SaulGoodmanBot.Commands;
using SaulGoodmanBot.Handlers;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands.Attributes;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot;

internal class Program {
    // Discord Client Properties
    public static DiscordClient? Client { get; private set; }
    public static InteractivityExtension? Interactivity { get; private set; }
    public static CommandsNextExtension? Commands { get; private set; }

    internal async Task MainAsync() {
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
            MinimumLogLevel = LogLevel.Debug,
        };

        Client = new DiscordClient(discordConfig);
        Client.UseInteractivity(new InteractivityConfiguration() {
            Timeout = TimeSpan.FromMinutes(2)
        });

        // Event Handlers
        Client.SessionCreated += GeneralHandlers.HandleOnReady;
        Client.GuildMemberAdded += GeneralHandlers.HandleMemberJoin;
        Client.GuildMemberRemoved += GeneralHandlers.HandleMemberLeave;
        Client.MessageCreated += BirthdayHandler.HandleBirthdayMessage;
        Client.MessageCreated += LevelHandler.HandleExpGain;
        Client.GuildRoleDeleted += RoleHandler.HandleServerRemoveRole;
        Client.GuildCreated += GeneralHandlers.HandleServerJoin;

        // Commands Config
        var commandsConfig = new CommandsNextConfiguration() {
            StringPrefixes = new string[] { configJSON.Prefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
        };

        // Prefix commands registration
        Commands = Client.UseCommandsNext(commandsConfig);

        // Slash commands registration
        var slash = Client.UseSlashCommands();
        slash.RegisterCommands<HelpCommands>();
        slash.RegisterCommands<MiscCommands>();
        slash.RegisterCommands<WheelPickerCommands>();
        slash.RegisterCommands<ReactionCommands>(270349691147780096);
        slash.RegisterCommands<BirthdayCommands>();
        slash.RegisterCommands<ServerConfigCommands>();
        slash.RegisterCommands<RoleCommands>();
        slash.RegisterCommands<LevelCommands>();
        slash.RegisterCommands<MinecraftCommands>();
        slash.RegisterCommands<ScheduleCommands>();

        // Secret Santa seasonal commands/handlers
        if (DateTime.Now.Month == 11 || DateTime.Now.Month == 12 || DateTime.Now.Month == 1) {
            Client.MessageCreated += SantaHandler.HandleParticipationDeadlineCheck;
            slash.RegisterCommands<SecretSantaCommands>();
        }

        slash.SlashCommandInvoked += async (s, e) => {
            Console.WriteLine($"{e.Context.CommandName} invoked by {e.Context.User} in {e.Context.Channel}/{e.Context.Guild}");
            await Task.CompletedTask;
        };

        slash.SlashCommandErrored += async (s, e) => {
            if (e.Exception is SlashExecutionChecksFailedException slex) {
                foreach (var check in slex.FailedChecks) {
                    if (check is SlashRequirePermissionsAttribute att)
                        await e.Context.CreateResponseAsync(StandardOutput.Error("Only an admin can run this command!"), ephemeral:true);
                }
            } else {
                await e.Context.CreateResponseAsync(StandardOutput.Error($"I'm not really sure what happened. Please let {Client.GetUserAsync(263070689559445504).Result.Mention} know!\nDebug info: `{e.Exception.Message}`"), ephemeral:true);
            }
        };

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    internal static void Main() => new Program().MainAsync().GetAwaiter().GetResult();
}

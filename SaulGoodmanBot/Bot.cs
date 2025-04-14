using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SaulGoodmanBot.Commands;
using SaulGoodmanBot.Handlers;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using SaulGoodmanLibrary;
using SaulGoodmanLibrary.Helpers;

namespace SaulGoodmanBot;

public class Bot 
{
    #region Discord Client Properties
    public static InteractivityExtension? Interactivity { get; private set; }
    public static CommandsNextExtension? Prefix { get; private set; }
    public static SlashCommandsExtension? Slash { get; private set; }
    public static System.Timers.Timer? Timer { get; private set; }
    public static Dictionary<DiscordGuild, ServerConfig> ServerConfig { get; private set; } = new();
    #endregion

    internal async Task Run() 
    {
        #region Discord Client Config
        var discordConfig = new DiscordConfiguration() 
        {
            Intents = DiscordIntents.All,
            Token = _token ?? throw new Exception("No token"),
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
            MinimumLogLevel = LogLevel.Debug,
        };

        DiscordHelper.Client = new DiscordClient(discordConfig);
        DiscordHelper.Client.UseInteractivity(new InteractivityConfiguration() 
        {
            Timeout = TimeSpan.FromMinutes(2)
        });
        Prefix = DiscordHelper.Client.UseCommandsNext(new CommandsNextConfiguration() 
        {
            StringPrefixes = new[] { "`" }

        });
        Prefix.RegisterCommands<SecretCommands>();
        #endregion

        #region Event Handler Registration
        DiscordHelper.Client.GuildMemberAdded += GeneralHandlers.HandleMemberJoin;
        DiscordHelper.Client.GuildMemberRemoved += GeneralHandlers.HandleMemberLeave;
        DiscordHelper.Client.MessageCreated += LevelHandler.HandleExpGain;
        DiscordHelper.Client.GuildRoleDeleted += RoleHandler.HandleServerRemoveRole;
        DiscordHelper.Client.GuildCreated += GeneralHandlers.HandleServerJoin;
        DiscordHelper.Client.ScheduledGuildEventCreated += GuildEventHandler.HandleGuildEventCreate;
        DiscordHelper.Client.Heartbeated += async (s, e) =>
        {
            DiscordHelper.ServerConfigs = new Dictionary<DiscordGuild, ServerConfig>();
            await foreach (var g in s.GetGuildsAsync())
            {
                DiscordHelper.ServerConfigs.Add(g, new ServerConfig(g));
            }

            await Task.CompletedTask;
        };
        #endregion

        #region Slash Commands
        Slash = DiscordHelper.Client.UseSlashCommands();
        //Slash.RegisterCommands<HelpCommands>();
        Slash.RegisterCommands<MiscCommands>();
        Slash.RegisterCommands<WheelPickerCommands>();
        Slash.RegisterCommands<ReactionCommands>(270349691147780096);
        Slash.RegisterCommands<BirthdayCommands>();
        Slash.RegisterCommands<ServerConfigCommands>();
        Slash.RegisterCommands<RoleCommands>();
        Slash.RegisterCommands<LevelCommands>();
        Slash.RegisterCommands<GuildEventCommands>();
        Slash.RegisterCommands<IdentityCommands>();

        // Secret Santa seasonal commands/handlers
        if (DateTime.Now.Month == 11 || DateTime.Now.Month == 12 || DateTime.Now.Month == 1) 
        {
            DiscordHelper.Client.MessageCreated += SantaHandler.HandleParticipationDeadlineCheck;
            Slash.RegisterCommands<SecretSantaCommands>();
        }

        Slash.SlashCommandExecuted += async (s, e) => { ServerConfig[e.Context.Guild] = new ServerConfig(e.Context.Guild); await Task.CompletedTask; };

        Slash.SlashCommandInvoked += async (s, e) => {
            Console.WriteLine($"{e.Context.CommandName} invoked by {e.Context.User} in {e.Context.Channel}/{e.Context.Guild}");
            await Task.CompletedTask;
        };

        Slash.SlashCommandErrored += async (s, e) => {
            if (e.Exception is SlashExecutionChecksFailedException slex) {
                foreach (var check in slex.FailedChecks) {
                    if (check is SlashRequirePermissionsAttribute att)
                        await e.Context.CreateResponseAsync(StandardOutput.Error("Only a server admin can run this command!"), ephemeral:true);
                }
            }

            if (e.Exception is Exception ex) 
            {
                var embed = new DiscordEmbedBuilder()
                    .WithAuthor("Error", "", ImageHelper.Images["Error"])
                    .AddField("Message", ex.Message)
                    .WithColor(DiscordColor.Red)
                    .WithThumbnail(ImageHelper.Images["Finger"]);

                await e.Context.CreateResponseAsync(embed, ephemeral:true);

#if DEBUG
                var me = await e.Context.Guild.GetMemberAsync(_loguigi);
                var dm = await me.CreateDmChannelAsync();
                embed.AddField("Command", e.Context.CommandName);
                embed.AddField("Source", ex.Source ?? "Unknown");
                embed.AddField("Invoked By", e.Context.User.Mention);

                await dm.SendMessageAsync(embed);       
#endif
            }
        };
        #endregion

        await DiscordHelper.Client.ConnectAsync();
        DiscordHelper.ServerConfigs = new Dictionary<DiscordGuild, ServerConfig>();
        Timer = new System.Timers.Timer(3600000) { AutoReset = true };
        Timer.Elapsed += async (s, e) => await BirthdayHandler.HandleBirthdayMessage(s, e);
        Timer.Start();
        await Task.Delay(-1);
    }

    internal static void Main() => new Bot().Run().GetAwaiter().GetResult();

    private string _token = Environment.GetEnvironmentVariable("SAULTOKEN") ?? throw new Exception("Token not set");
    private ulong _loguigi = ulong.Parse(Environment.GetEnvironmentVariable("LOGUIGI") ?? throw new Exception("Loguigi UID not set"));
}

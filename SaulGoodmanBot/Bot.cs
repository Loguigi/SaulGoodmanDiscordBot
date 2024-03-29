using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using SaulGoodmanBot.Config;
using SaulGoodmanBot.Commands;
using SaulGoodmanBot.Handlers;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands.Attributes;
using SaulGoodmanBot.Helpers;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using SaulGoodmanBot.Controllers;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot;

public class Bot {
    #region Discord Client Properties
    public static DiscordClient? Client { get; private set; }
    public static InteractivityExtension? Interactivity { get; private set; }
    public static CommandsNextExtension? Prefix { get; private set; }
    public static SlashCommandsExtension? Slash { get; private set; }
    public static System.Timers.Timer? Timer { get; private set; }
    public static Dictionary<DiscordGuild, ServerConfig> ServerConfig { get; private set; } = new();
    #endregion

    internal async Task Run() {
        Env.SetContext();
        using System.Timers.Timer Timer = new(TimeSpan.FromHours(1));

        #region Discord Client Config
        var discordConfig = new DiscordConfiguration() {
            Intents = DiscordIntents.All,
            Token = Env.Token ?? throw new Exception("No token"),
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
            MinimumLogLevel = LogLevel.Debug,
        };

        Client = new DiscordClient(discordConfig);
        Client.UseInteractivity(new InteractivityConfiguration() {
            Timeout = TimeSpan.FromMinutes(2)
        });
        Prefix = Client.UseCommandsNext(new CommandsNextConfiguration() {
            StringPrefixes = new[] { "`" }

        });
        Prefix.RegisterCommands<SecretCommands>();
        #endregion

        #region Event Handler Registration
        Client.SessionCreated += async (s, e) => {
            foreach (var g in Client.Guilds.Values) {
                ServerConfig.Add(g, new ServerConfig(g));
            }
            await Task.CompletedTask;
        };
        Client.GuildMemberAdded += GeneralHandlers.HandleMemberJoin;
        Client.GuildMemberRemoved += GeneralHandlers.HandleMemberLeave;
        Client.MessageCreated += LevelHandler.HandleExpGain;
        Client.GuildRoleDeleted += RoleHandler.HandleServerRemoveRole;
        Client.GuildCreated += GeneralHandlers.HandleServerJoin;
        Client.ScheduledGuildEventCreated += GuildEventHandler.HandleGuildEventCreate;
        Timer.Elapsed += async (s, e) => await BirthdayHandler.HandleBirthdayMessage(e);
        #endregion

        #region Slash Commands
        Slash = Client.UseSlashCommands();
        Slash.RegisterCommands<HelpCommands>();
        Slash.RegisterCommands<MiscCommands>();
        Slash.RegisterCommands<WheelPickerCommands>();
        Slash.RegisterCommands<ReactionCommands>(270349691147780096);
        Slash.RegisterCommands<BirthdayCommands>();
        Slash.RegisterCommands<ServerConfigCommands>();
        Slash.RegisterCommands<RoleCommands>();
        Slash.RegisterCommands<LevelCommands>();
        //Slash.RegisterCommands<MinecraftCommands>();
        Slash.RegisterCommands<ScheduleCommands>();
        Slash.RegisterCommands<GuildEventCommands>();

        // Secret Santa seasonal commands/handlers
        if (DateTime.Now.Month == 11 || DateTime.Now.Month == 12 || DateTime.Now.Month == 1) {
            //Client.MessageCreated += SantaHandler.HandleParticipationDeadlineCheck;
            //Slash.RegisterCommands<SecretSantaCommands>();
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

            if (e.Exception is Exception ex) {
                var embed = new DiscordEmbedBuilder()
                    .WithAuthor("Error", "", ImageHelper.Images["Error"])
                    .AddField("Message", ex.Message)
                    .AddField("Source", ex.Source ?? "Unknown")
                    .WithColor(DiscordColor.Red)
                    .WithThumbnail(ImageHelper.Images["Finger"]);

                await e.Context.CreateResponseAsync(embed, ephemeral:true);

                if (Env.DebugMode) {
                    var me = await e.Context.Guild.GetMemberAsync(Env.Loguigi);
                    var dm = await me.CreateDmChannelAsync();
                    embed.AddField("Command", e.Context.CommandName);
                    embed.AddField("Source", ex.Source ?? "Unknown");
                    embed.AddField("Invoked By", e.Context.User.Mention);

                    await dm.SendMessageAsync(embed);
                }
            }
        };
        #endregion

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    internal static void Main() => new Bot().Run().GetAwaiter().GetResult();
}

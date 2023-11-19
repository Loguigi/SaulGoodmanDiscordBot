using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library.SecretSanta;
using SaulGoodmanBot.Library.Helpers;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("santa", "Commands to manage a Secret Santa gift exchange")]
[GuildOnly]
public class SecretSantaCommands : ApplicationCommandModule {
    [SlashCommandGroup("event", "Commands for the event itself")]
    public class EventCommmands : ApplicationCommandModule {
        [SlashCommand("start", "Starts the event and sends a notification to the chat")]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task StartEvent(InteractionContext ctx,
            [ChoiceProvider(typeof(WinterMonthChoiceProvider))][Option("participation_deadline_month", "Month for the participation deadline")] long participation_month,
            [Option("participation_deadline_day", "Day for the participation deadline")][Minimum(1)][Maximum(31)] long participation_day,
            [ChoiceProvider(typeof(WinterMonthChoiceProvider))][Option("exchange_month", "Month for the gift exchange")] long exchange_month,
            [Option("exchange_day", "Day for the gift exchange")] long exchange_day,
            [Option("exchange_location", "Location for the gift exchange")][MaximumLength(30)] string exchange_location,
            [Option("price_limit", "Price limit to set for gifts (optional)")] double? limit=null) {
            
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Event has already started"), ephemeral:true);
                return;
            }

            if ((participation_month == 11 && participation_day == 31) || (exchange_month == 11 && exchange_day == 31)) {
                await ctx.CreateResponseAsync(StandardOutput.Error("November 31 does not exist"), ephemeral:true);
                return;
            }

            var participation_deadline = new DateTime(participation_month == 1 ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year, (int)participation_month, (int)participation_day);
            var exchange_date = new DateTime(exchange_month == 1 ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year, (int)exchange_month, (int)exchange_day);

            if (!ValidateDates(participation_deadline, exchange_date)) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Date error. The participation deadline date must be after today and before the exchange date"));
                return;
            }

            santa.StartEvent(new SantaConfig(ctx.Guild) {
                ParticipationDeadline = participation_deadline,
                ExchangeDate = exchange_date,
                ExchangeLocation = exchange_location,
                PriceLimit = limit
            });

            // TODO design output
        }

        [SlashCommand("participate", "Participate in the Secret Santa")]
        public async Task Participate(InteractionContext ctx,
            [Option("first_name", "Please put your real first name")][MaximumLength(20)] string name) {
            
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Participation deadline has passed!"), ephemeral:true);
                return;
            }

            if (santa.Find(ctx.User) != null) {
                await ctx.CreateResponseAsync(StandardOutput.Error("You are already participating!"), ephemeral:true);
                return;
            }

            santa.AddParticipant(ctx.User, name);

            // TODO design output
        }

        [SlashCommand("lock", "Force locks all participants before the deadline and begins name assignment")]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task LockParticipants(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Gifting has already begun"), ephemeral:true);
                return;
            }

            if (santa.Participants.Count < 3) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Cannot start gifting phase. Needs at least 3 participants"), ephemeral:true);
            }

            santa.AssignNames();

            // TODO design output
        }
    }

    [SlashCommandGroup("view", "Commands to view information for the Secret Santa")]
    public class ViewCommands : ApplicationCommandModule {
        public async Task ViewGiftee(InteractionContext ctx) {

        }
    }

    [SlashCommandGroup("config", "Configuration for the Secret Santa")]
    public class ConfigCommands : ApplicationCommandModule {
        public async Task SetCouple(InteractionContext ctx,
            [Option("first", "First person")] DiscordUser user1,
            [Option("second", "Second person")] DiscordUser user2) {

            var santa = new Santa(ctx.Client, ctx.Guild);

            if (santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Names have already been assignd"), ephemeral:true);
                return;
            }

            if (user1 == user2) {
                await ctx.CreateResponseAsync(StandardOutput.Error("What are you doing"));
                return;
            }

            var participant1 = santa.Find(user1);
            var participant2 = santa.Find(user2);

            if (participant1 == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error($"{user1.Mention} has not chosen to participate yet"), ephemeral:true);
                return;
            } 
            
            if (participant2 == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error($"{user2.Mention} has not chosen to participate yet"), ephemeral:true);
                return;
            }

            santa.AddCouple(participant1, participant2);
            
            // TODO design output
        }
    }

    private static bool ValidateDates(DateTime participation_deadline, DateTime exchange) {
        return exchange > participation_deadline && participation_deadline >= DateTime.Today;
    }
}

public class WinterMonthChoiceProvider : IChoiceProvider
{
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
        await Task.CompletedTask;
        return new DiscordApplicationCommandOptionChoice[]
        {
            new("January", 1),
            new("November", 11),
            new("December", 12),
        };
    }
}
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library.SecretSanta;
using SaulGoodmanBot.Library.Helpers;
using SaulGoodmanBot.Handlers;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands.Attributes;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("santa", "Commands to manage a Secret Santa gift exchange")]
public class SecretSantaCommands : ApplicationCommandModule {
    [SlashCommandGroup("event", "Commands for the event itself")]
    public class EventCommmands : ApplicationCommandModule {
        [SlashCommand("start", "Starts the event and sends a notification to the chat")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task StartEvent(InteractionContext ctx,
            [ChoiceProvider(typeof(WinterMonthChoiceProvider))][Option("participation_deadline_month", "Month for the participation deadline")] long participation_month,
            [Option("participation_deadline_day", "Day for the participation deadline")][Minimum(1)][Maximum(31)] long participation_day,
            [ChoiceProvider(typeof(WinterMonthChoiceProvider))][Option("exchange_month", "Month for the gift exchange")] long exchange_month,
            [Option("exchange_day", "Day for the gift exchange")][Minimum(1)][Maximum(31)] long exchange_day,
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

            if (!ValidateDates(participation_deadline, exchange_date, false)) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Date error. The participation deadline date must be after today and before the exchange date"));
                return;
            }

            santa.StartEvent(new SantaConfig(ctx.Guild) {
                SantaRole = await ctx.Guild.CreateRoleAsync("Secret Santa", null, DiscordColor.DarkRed, null),
                ParticipationDeadline = participation_deadline,
                ExchangeDate = exchange_date,
                ExchangeLocation = exchange_location,
                PriceLimit = limit
            });

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle($"{DateTime.Now.Year} Secret Santa")
                .WithDescription($"Welcome to this year's Secret Santa!\n\nTo participate, type </santa event participate:1177333432649531493>. Once the participation deadline has past **({santa.Config.ParticipationDeadline:dddd, MMMM d})** or an admin locks in the names, you can type </santa view giftee:1177333432649531493> to see the name of the person you are gifting.\n\nYou can enter the other `/santa view` commands to see these gift exchange details again.")
                .WithThumbnail(ImageHelper.Images["WalterChristmas"])
                .AddField("Date", $"{santa.Config.ExchangeDate:dddd, MMMM d}", true)
                .AddField("Location", santa.Config.ExchangeLocation, true)
                .AddField("Price Limit", santa.Config.PriceLimit?.ToString("C2") ?? "None", true)
                .WithColor(DiscordColor.SapGreen)
                .WithFooter("Merry Christmas!");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed)));
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
            await ctx.Member.GrantRoleAsync(santa.Config.SantaRole);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Thank you!")
                .WithDescription($"After {santa.Config.ParticipationDeadline:MMMM d} has passed, you can enter </santa view giftee:1177333432649531493> to see who you are gifting\n\nAs a reminder, here are the gift exchange details:")
                .AddField("Date", $"{santa.Config.ExchangeDate:dddd, MMMM d}", true)
                .AddField("Location", santa.Config.ExchangeLocation, true)
                .AddField("Price Limit", santa.Config.PriceLimit?.ToString("C2") ?? "None", true)
                .WithColor(DiscordColor.Azure);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
        }

        [SlashCommand("lock", "Force locks all participants before the deadline and begins name assignment")]
        [SlashRequirePermissions(Permissions.Administrator)]
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

            var embed = new DiscordEmbedBuilder()
                .WithAuthor("ATTENTION", "https://youtu.be/a3_PPdjD6mg?si=4q_PpummrNXtmZmP", ImageHelper.Images["Heisenberg"])
                .WithTitle("Participation has ended")
                .WithDescription("### </santa view giftee:1177333432649531493> to see who you're gifting!")
                .WithColor(DiscordColor.Orange)
                .WithFooter("(Nobody else will see)");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("@everyone").AddMention(new EveryoneMention()).AddEmbed(embed)));
        }

        [SlashCommand("end", "End the Secret Santa event")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task EndEvent(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("The event has to start before it can end"), ephemeral:true);
                return;
            }

            santa.EndEvent();

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent($"{santa.Config.SantaRole.Mention} has ended").AddMention(new RoleMention(santa.Config.SantaRole))));
            await ctx.Guild.GetRole(santa.Config.SantaRole.Id).DeleteAsync();
        }
    }

    [SlashCommandGroup("gift", "Commands to manage Secret Santa gifts")]
    public class GiftCommands : ApplicationCommandModule {
        [SlashCommand("ready", "Use this command if you've gotten your gift for your secret Santa")]
        public async Task GiftReady(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);
            var user = santa.Find(ctx.User);

            if (user == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error("You must participate first using </santa event participate:1177333432649531493> before getting a gift"), ephemeral:true);
                return;
            }

            if (!santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Please wait for names to be assigned before getting your gift"), ephemeral:true);
                return;
            }

            user.SetGiftReady(ctx.Guild.Id);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Thank you!")
                .WithDescription(user.GiftReady ? "When everyone has their gift, the gift exchange can begin" : "When you have your gift, enter this command again to change")
                .WithColor(DiscordColor.PhthaloGreen)
                .WithFooter(user.GiftReady ? $"{DiscordEmoji.FromName(ctx.Client, ":white_check_mark:", false)} Ready" : $"{DiscordEmoji.FromName(ctx.Client, ":x:", false)} Not Ready");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
        }

        [SlashCommand("statuses", "See if everyone is ready with their gift or not")]
        public async Task GiftStatuses(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Please wait for names to be assigned first"), ephemeral:true);
                return;
            }

            var interactivity = new InteractivityHelper<SantaParticipant>(ctx.Client, santa.Participants, IDHelper.Santa.GIFTSTATUSES, "1");
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Secret Santa Gift Statuses")
                .WithDescription("List of people that have gifts ready for their Secret Santa\n\n")
                .WithColor(DiscordColor.Rose)
                .WithFooter(interactivity.PageStatus());

            foreach (var p in interactivity.GetPage()) {
                embed.Description += $"{(p.GiftReady ? DiscordEmoji.FromName(ctx.Client, ":white_check_mark:", false) : DiscordEmoji.FromName(ctx.Client, ":x:", false))} {p.User.Mention} ({p.FirstName}) {(p.GiftReady ? "`READY`" : "`NOT READY`")}\n";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

            ctx.Client.ComponentInteractionCreated -= SantaHandler.HandleGiftStatusesList;
            ctx.Client.ComponentInteractionCreated += SantaHandler.HandleGiftStatusesList;
        }
    }

    [SlashCommandGroup("view", "Commands to view information for the Secret Santa")]
    public class ViewCommands : ApplicationCommandModule {
        [SlashCommand("giftee", "View the person you are gifting")]
        public async Task ViewGiftee(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);
            var gifter = santa.Find(ctx.User);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("You are gifting...")
                .WithDescription("")
                .WithColor(DiscordColor.Rose);

            if (gifter == null) {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(embed.WithDescription("You must participate using </santa event participate:1177333432649531493> before you can receive a name!"))).AsEphemeral());
                return;
            }

            if (!santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(embed.WithDescription("Names have not been drawn yet! Please check back after the participation deadline"))).AsEphemeral());
                return;
            }

            var giftee = santa.Find(gifter.Giftee ?? throw new ArgumentNullException(nameof(gifter.Giftee)));

            embed.WithDescription($"# {giftee!.User.Mention} ({giftee!.FirstName})").WithThumbnail(giftee!.User.AvatarUrl).WithFooter("PLEASE DON'T TELL ANYBODY!!!");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(embed)).AsEphemeral());
        }

        [SlashCommand("participants", "View everyone that is participating in the Secret Santa")]
        public async Task ViewParticipants(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);
            var interactivity = new InteractivityHelper<SantaParticipant>(ctx.Client, santa.Participants, IDHelper.Santa.PARTICIPANTS, "1", "There are no participants yet");

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Secret Santa Participants")
                .WithDescription(interactivity.IsEmpty())
                .WithThumbnail(ImageHelper.Images["BetterCallSanta"])
                .WithColor(DiscordColor.SapGreen)
                .WithFooter(interactivity.PageStatus());

            foreach (var p in interactivity.GetPage()) {
                embed.Description += $"{p.User.Mention} ({p.FirstName})\n";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));

            ctx.Client.ComponentInteractionCreated -= SantaHandler.HandleParticipantList;
            ctx.Client.ComponentInteractionCreated += SantaHandler.HandleParticipantList;
        }

        [SlashCommand("exchange_details", "View the time and location of the gift exchange")]
        public async Task ViewExchangeDetails(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Gift Exchange Details")
                .WithColor(DiscordColor.Rose)
                .WithThumbnail(ImageHelper.Images["WalterChristmas"])
                .AddField("Date/Time", santa.Config.HasStarted ? $"{santa.Config.ExchangeDate:dddd, MMMM d h:mm tt}" : "Undecided")
                .AddField("Location", santa.Config.HasStarted ? $"{santa.Config.ExchangeLocation}" : "Undecided");

            if (santa.Config.ExchangeAddress != null)
                embed.AddField("Address", santa.Config.ExchangeAddress);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(embed)));
        }

        [SlashCommand("price_limit", "View the price limit if there is one")]
        public async Task ViewPriceLimit(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);
            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, "", ctx.Guild.IconUrl)
                .WithTitle("Gift Price Limit")
                .WithDescription(santa.Config.HasStarted ? (santa.Config.PriceLimit.ToString() ?? "### No price limit") : "### Undecided")
                .WithColor(DiscordColor.Green)
                .WithThumbnail(ImageHelper.Images["WalterChristmas"]);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithEmbed(embed)));
        }
    }

    [SlashCommandGroup("wishlist", "Secret Santa wishlist")]
    public class WishlistCommands : ApplicationCommandModule {
        [SlashCommand("add", "Add items to your wishlist")]
        public async Task WishlistAdd(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);
            var interactivity = ctx.Client.GetInteractivity();

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Secret Santa has not started yet. Please try again after"), ephemeral:true);
                return;
            }

            var user = santa.Find(ctx.User);

            if (user == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error("You must participate using </santa event participate:1177333432649531493> before creating a wishlist"), ephemeral:true);
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(user.User.GlobalName, "", user.User.AvatarUrl)
                .WithTitle($"Adding to wishlist...")
                .WithDescription($"Type an item to add or `stop` to stop adding\nTo add multiple items, separate by a new line\nMaximum of {SantaParticipant.MAX_WISHLIST_ITEMS} items allowed")
                .AddField("Last Added", "---", false)
                .AddField("Status", "Still listening", true)
                .AddField("Total Items", user.Wishlist.Count.ToString(), true);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
            var response = await interactivity.WaitForMessageAsync(u => u.Channel == ctx.Channel && u.Author == ctx.User, TimeSpan.FromSeconds(300));

            while (!response.Result.Content.ToLower().Contains("stop") && !user.IsWishlistFull() && !response.TimedOut) {
                if (response.Result.Content.Contains('\n')) {
                    foreach (var i in response.Result.Content.Split('\n')) {
                        if (user.IsWishlistFull())
                            break;
                        user.EditWishlist(ctx.Guild.Id, DataOperations.Add, i);
                        user.Wishlist.Add(i);
                        embed.Fields.Where(x => x.Name == "Last Added").First().Value = i;
                    }
                } else {
                    var item = response.Result.Content;
                    user.EditWishlist(ctx.Guild.Id, DataOperations.Add, item);
                    user.Wishlist.Add(item);
                    embed.Fields.Where(x => x.Name == "Last Added").First().Value = item;
                }
                
                embed.Fields.Where(x => x.Name == "Total Items").First().Value = user.Wishlist.Count.ToString();
                await ctx.Channel.DeleteMessageAsync(response.Result);
                await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                response = await interactivity.WaitForMessageAsync(u => u.Channel == ctx.Channel && u.Author == ctx.User, TimeSpan.FromSeconds(300));
            }

            embed.Fields.Where(x => x.Name == "Status").First().Value = "Finished";
            embed.WithColor(DiscordColor.Green);
            await ctx.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }

        [SlashCommand("remove", "Remove items from your wishlist")]
        public async Task WishlistRemove(InteractionContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Secret Santa has not started yet. Please try again after"), ephemeral:true);
                return;
            }

            var user = santa.Find(ctx.User);

            if (user == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error("You must participate using </santa event participate:1177333432649531493> before creating a wishlist"), ephemeral:true);
                return;
            }

            if (user.Wishlist.Count == 0) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Your wishlist is empty!"), ephemeral:true);
                return;
            }

            var dropdown_options = new List<DiscordSelectComponentOption>() {new DiscordSelectComponentOption("Cancel", "CANCEL", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":arrow_backward:", false)))};
            foreach (var i in user.Wishlist) {
                dropdown_options.Add(new DiscordSelectComponentOption(i, i, "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":gift:", false))));
            }
            var dropdown = new DiscordSelectComponent(IDHelper.Santa.WISHLISTREMOVE, "Select a wishlist item...", dropdown_options, false, 1, user.Wishlist.Count + 1);

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Wishlist Remove")
                .WithColor(DiscordColor.DarkRed);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(dropdown)));

            ctx.Client.ComponentInteractionCreated -= SantaHandler.HandleWishlistRemove;
            ctx.Client.ComponentInteractionCreated += SantaHandler.HandleWishlistRemove;
        }

        [SlashCommand("view", "View your own or somebody else's wishlist")]
        public async Task WishlistView(InteractionContext ctx,
            [Option("user", "Person's wishlist that you want to see")] DiscordUser? user=null) {
            
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (user! != null! && user.IsBot) {
                await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
                return;
            }

            user ??= ctx.User;

            var participant = santa.Find(user);

            if (participant == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error($"{user.Mention} has not chosen to participate in the Secret Santa"), ephemeral:true);
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(user.GlobalName, "", user.AvatarUrl)
                .WithTitle("Secret Santa Wishlist")
                .WithDescription(participant.Wishlist.Count == 0 ? $"### {user.Mention}'s wishlist is empty" : "")
                .WithColor(DiscordColor.Teal);

            foreach (var i in participant.Wishlist) {
                embed.Description += $"* {i}\n";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "Secret Santa wishlist")]
        public async Task ContextWishlistView(ContextMenuContext ctx) {
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (ctx.TargetUser.IsBot) {
                await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
                return;
            }

            var participant = santa.Find(ctx.TargetUser);

            if (participant == null) {
                await ctx.CreateResponseAsync(StandardOutput.Error($"{ctx.TargetUser.Mention} has not chosen to participate in the Secret Santa"), ephemeral:true);
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(ctx.TargetUser.GlobalName, "", ctx.TargetUser.AvatarUrl)
                .WithTitle("Secret Santa Wishlist")
                .WithDescription(participant.Wishlist.Count == 0 ? $"### {ctx.TargetUser.Mention}'s wishlist is empty" : "")
                .WithColor(DiscordColor.Teal);

            foreach (var i in participant.Wishlist) {
                embed.Description += $"* {i}\n";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)).AsEphemeral());
        }
    }

    [SlashCommandGroup("config", "Configuration for the Secret Santa")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public class ConfigCommands : ApplicationCommandModule {
        [SlashCommand("setcouple", "Set 2 people to be a couple so they don't get each others' names")]
        public async Task SetCouple(InteractionContext ctx,
            [Option("first", "First person")] DiscordUser user1,
            [Option("second", "Second person")] DiscordUser user2) {

            var santa = new Santa(ctx.Client, ctx.Guild);

            if (santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Names have already been assigned"), ephemeral:true);
                return;
            }

            if (user1 == user2) {
                await ctx.CreateResponseAsync(StandardOutput.Error("What are you doing"), ephemeral:true);
                return;
            }

            if (user1.IsBot) {
                await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
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
            
            await ctx.CreateResponseAsync(StandardOutput.Success($"{participant1.User.Mention} and {participant2.User.Mention} will not get each others' names"), ephemeral:true);
        }

        [SlashCommand("add_participant", "Add a participant manually")]
        public async Task AddParticipant(InteractionContext ctx,
            [Option("user", "User to add")] DiscordUser user,
            [Option("name", "First name of the user")] string name) {
            
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("The event must be started using </santa event start:1177333432649531493> before details can be changed"), ephemeral:true);
                return;
            }

            if (santa.Config.LockedIn) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Names have already been assigned so nobody else can be added"), ephemeral:true);
                return;
            }

            if (user.IsBot) {
                await ctx.CreateResponseAsync("https://tenor.com/view/saul-goodman-better-call-saul-saul-goodman3d-meme-breaking-bad-gif-24027228");
                return;
            }

            if (santa.Find(user) != null) {
                await ctx.CreateResponseAsync(StandardOutput.Error($"{user.Mention} is already participating"), ephemeral:true);
                return;
            }

            santa.AddParticipant(user, name);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent($"{user.Mention} has been added to the Secret Santa!").AddMention(new UserMention(user))));
        }

        [SlashCommand("exchange_date", "Change the date of the gift exchange")]
        public async Task SetExchangeDate(InteractionContext ctx,
            [ChoiceProvider(typeof(WinterMonthChoiceProvider))][Option("month", "Month of gift exchange")] long month,
            [Option("day", "Day of gift exchange")][Minimum(1)][Maximum(31)] long day,
            [Option("hour", "Hour of when the gift exchange starts")][Minimum(0)][Maximum(23)] long hour=0,
            [Option("minute", "Minute of when the gift exchange starts")][Minimum(0)][Maximum(59)] long minute=0) {

            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("The event must be started using </santa event start:1177333432649531493> before details can be changed"), ephemeral:true);
                return;
            }

            if (month == 11 && day == 31) {
                await ctx.CreateResponseAsync(StandardOutput.Error("November 31 does not exist"), ephemeral:true);
                return;
            }

            var exchange_date = new DateTime(month == 1 ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year, (int)month, (int)day, (int)hour, (int)minute, 0);

            if (!ValidateDates(santa.Config.ParticipationDeadline, exchange_date, santa.Config.LockedIn)) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Date error. The participation deadline date must be after today and before the exchange date"));
                return;
            }

            santa.Config.ExchangeDate = exchange_date;
            santa.Config.Update();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor("ATTENTION", "https://youtu.be/a3_PPdjD6mg?si=4q_PpummrNXtmZmP", ImageHelper.Images["Heisenberg"])
                .WithTitle("The gift exchange date/time has been changed to")
                .WithDescription($"# {santa.Config.ExchangeDate:dddd, MMMM d h:mm tt}")
                .WithColor(DiscordColor.Yellow);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent(santa.Config.SantaRole.Mention).AddMention(new RoleMention(santa.Config.SantaRole)).AddEmbed(embed)));
        }

        [SlashCommand("exchange_location", "Change the location of the gift exchange")]
        public async Task SetExchangeLocation(InteractionContext ctx,
            [Option("location", "Location of the gift exchange")][MaximumLength(30)] string location,
            [Option("address", "Address of the gift exchange location")][MaximumLength(100)] string? address=null) {
            
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("The event must be started using </santa event start:1177333432649531493> before details can be changed"), ephemeral:true);
                return;
            }

            santa.Config.ExchangeLocation = location;
            santa.Config.ExchangeAddress = address;
            santa.Config.Update();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor("ATTENTION", "https://youtu.be/a3_PPdjD6mg?si=4q_PpummrNXtmZmP", ImageHelper.Images["Heisenberg"])
                .WithTitle("The gift exchange location has been changed to")
                .WithDescription($"# {santa.Config.ExchangeLocation}")
                .WithColor(DiscordColor.Yellow);
            
            if (santa.Config.ExchangeAddress != null) {
                embed.AddField("Address", santa.Config.ExchangeAddress);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent(santa.Config.SantaRole.Mention).AddMention(new RoleMention(santa.Config.SantaRole)).AddEmbed(embed)));
        }

        [SlashCommand("participation_deadline_date", "Change the date of the participation deadline")]
        public async Task SetParticipationDeadlineDate(InteractionContext ctx,
            [ChoiceProvider(typeof(WinterMonthChoiceProvider))][Option("month", "Month of gift exchange")] long month,
            [Option("day", "Day of gift exchange")][Minimum(1)][Maximum(31)] long day) {
            
            var santa = new Santa(ctx.Client, ctx.Guild);

            if (!santa.Config.HasStarted) {
                await ctx.CreateResponseAsync(StandardOutput.Error("The event must be started using </santa event start:1177333432649531493> before details can be changed"), ephemeral:true);
                return;
            }

            if (month == 11 && day == 31) {
                await ctx.CreateResponseAsync(StandardOutput.Error("November 31 does not exist"), ephemeral:true);
                return;
            }

            var participation_deadline = new DateTime(month == 1 ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year, (int)month, (int)day);

            if (!ValidateDates(participation_deadline, santa.Config.ExchangeDate, santa.Config.LockedIn)) {
                await ctx.CreateResponseAsync(StandardOutput.Error("Date error. The participation deadline date must be after today and before the exchange date"));
                return;
            }

            santa.Config.ParticipationDeadline = participation_deadline;
            santa.Config.Update();

            var embed = new DiscordEmbedBuilder()
                .WithAuthor("ATTENTION", "https://youtu.be/a3_PPdjD6mg?si=4q_PpummrNXtmZmP", ImageHelper.Images["Heisenberg"])
                .WithTitle("The participation deadline date has been changed to")
                .WithDescription($"# {santa.Config.ParticipationDeadline:dddd, MMMM d}")
                .WithColor(DiscordColor.Yellow);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent(santa.Config.SantaRole.Mention).AddMention(new RoleMention(santa.Config.SantaRole)).AddEmbed(embed)));
        }
    }

    private static bool ValidateDates(DateTime participation_deadline, DateTime exchange_date, bool locked_in) {
        return exchange_date > participation_deadline && (locked_in || participation_deadline >= DateTime.Today);
    }
}

public class WinterMonthChoiceProvider : IChoiceProvider {
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() {
        await Task.CompletedTask;
        return new DiscordApplicationCommandOptionChoice[] {
            new("January", 1),
            new("November", 11),
            new("December", 12),
        };
    }
}
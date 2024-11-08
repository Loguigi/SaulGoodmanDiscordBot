using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using SaulGoodmanBot.Helpers;
using SaulGoodmanBot.Controllers;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Handlers;

public static class SantaHandler 
{
    public static async Task HandleParticipationDeadlineCheck(DiscordClient s, MessageCreateEventArgs e) 
    {
        var santa = new Santa(s, e.Guild);

        if (!santa.Config.HasStarted || santa.Config.LockedIn || e.Message.CreationTimestamp.LocalDateTime != santa.Config.ParticipationDeadline) 
        {
            await Task.CompletedTask;
            return;
        }

        var config = new ServerConfig(e.Guild);

        if (santa.NotEnoughParticipants) 
        {
            await config.DefaultChannel.SendMessageAsync("Not enough participants for Secret Santa. Changing deadline to tomorrow");
            santa.Config.ParticipationDeadline = santa.Config.ParticipationDeadline.AddDays(1);
            santa.Config.Update();
            return;
        }

        santa.AssignNames();

        var embed = new DiscordEmbedBuilder()
            .WithAuthor("ATTENTION", "https://youtu.be/a3_PPdjD6mg?si=4q_PpummrNXtmZmP", ImageHelper.Images["Heisenberg"])
            .WithTitle("Participation deadline has passed")
            .WithDescription("### </santa view giftee:1177333432649531493> to see who you're gifting!")
            .WithColor(DiscordColor.Orange)
            .WithFooter("(Nobody else will see)");

        await config.DefaultChannel.SendMessageAsync(new DiscordMessageBuilder().WithContent(e.Guild.EveryoneRole.Mention).AddEmbed(embed));
    }

    public static async Task HandleParticipantList(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Santa.PARTICIPANTS)) 
        {
            await Task.CompletedTask;
            return;
        }

        var santa = new Santa(s, e.Guild);
        var interactivity = new InteractivityHelper<SantaParticipant>(s, santa.Participants, IDHelper.Santa.PARTICIPANTS, e.Id.Split('\\')[PAGE_INDEX], 10, "There are no participants yet");

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
            .WithTitle("Secret Santa Participants")
            .WithDescription(interactivity.IsEmpty())
            .WithThumbnail(ImageHelper.Images["BetterCallSanta"])
            .WithColor(DiscordColor.SapGreen)
            .WithFooter(interactivity.PageStatus);

        foreach (var p in interactivity) 
        {
            embed.Description += $"{p.User.Mention} ({p.FirstName})\n";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
    }

    public static async Task HandleGiftStatusesList(DiscordClient s, ComponentInteractionCreateEventArgs e) 
    {
        if (!e.Id.Contains(IDHelper.Santa.GIFTSTATUSES)) {
            await Task.CompletedTask;
            return;
        }

        var santa = new Santa(s, e.Guild);
        var interactivity = new InteractivityHelper<SantaParticipant>(s, santa.Participants, IDHelper.Santa.GIFTSTATUSES, e.Id.Split('\\')[PAGE_INDEX], 10);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(e.Guild.Name, "", e.Guild.IconUrl)
            .WithTitle("Secret Santa Gift Statuses")
            .WithDescription("List of people that have gifts ready for their Secret Santa\n\n")
            .WithColor(DiscordColor.Rose)
            .WithFooter(interactivity.PageStatus);

        foreach (var p in interactivity) 
        {
            embed.Description += $"{(p.GiftReady ? DiscordEmoji.FromName(s, ":white_check_mark:", false) : DiscordEmoji.FromName(s, ":x:", false))} {p.User.Mention} ({p.FirstName}) {(p.GiftReady ? "`READY`" : "`NOT READY`")}\n";
        }

        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(interactivity.AddPageButtons().AddEmbed(embed)));
    }

    private const int PAGE_INDEX = 1;
}
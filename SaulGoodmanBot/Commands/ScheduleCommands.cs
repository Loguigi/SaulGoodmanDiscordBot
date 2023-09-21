using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("schedule", "Commands for saving users' work schedules to make hangout planning easier")]
[GuildOnly]
public class ScheduleCommands : ApplicationCommandModule {
    [SlashCommand("save", "Save a new schedule")]
    public async Task SaveSchedule(InteractionContext ctx,
        [Option("recurring", "False if your schedule changes every week, true if it does not change")] bool recurring,
        [Option("sunday", "Schedule for Sunday")] string? sunday=null,
        [Option("monday", "Schedule for Monday")] string? monday=null,
        [Option("tuesday", "Schedule for Tuesday")] string? tuesday=null,
        [Option("wednesday", "Schedule for Wednesday")] string? wednesday=null,
        [Option("thursday", "Schedule for Thursday")] string? thursday=null,
        [Option("friday", "Schedule for Friday")] string? friday=null,
        [Option("saturday", "Schedule for Saturday")] string? saturday=null,
        [Option("picture", "Picture of your schedule if you have one")] DiscordAttachment? picture=null) {

        var schedule = new Schedule(ctx.Guild, ctx.User) {
            LastUpdated = ctx.Interaction.CreationTimestamp.LocalDateTime,
            RecurringSchedule = recurring,
            WorkSchedule = {
                {DayOfWeek.Sunday, sunday},
                {DayOfWeek.Monday, monday},
                {DayOfWeek.Tuesday, tuesday},
                {DayOfWeek.Wednesday, wednesday},
                {DayOfWeek.Thursday, thursday},
                {DayOfWeek.Friday, friday},
                {DayOfWeek.Saturday, saturday}
            },
            PictureUrl = picture?.Url
        };
        schedule.Update();

        // TODO add output similar that shows user's schedule after creation
    }

    [SlashCommand("check", "Check your own or somebody else's schedule")]
    public async Task CheckSchedule(InteractionContext ctx,
        [Option("user", "Schedule of specific user")] DiscordUser? user=null) {
        
        var schedule = new Schedule(ctx.Guild, user ?? ctx.User);

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(schedule.User.GlobalName, "", schedule.User.AvatarUrl)
            .WithTitle("Work Schedule")
            .WithDescription(schedule.RecurringSchedule ? "Schedule does not change" : "Schedule changes weekly")
            .WithImageUrl(schedule.PictureUrl ?? "")
            .WithFooter($"Last updated {(schedule.LastUpdated != schedule.NO_DATE ? schedule.LastUpdated : "never")}")
            .WithColor(DiscordColor.Teal);

        foreach (var day in schedule.WorkSchedule) {
            if (day.Value != null) {
                embed.AddField(day.Key.ToString("ddd"), day.Value, true);
            }
        }

        if (!schedule.RecurringSchedule && DateTime.Now > schedule.LastUpdated.AddDays(7)) {
            embed.WithFooter($"{DiscordEmoji.FromName(ctx.Client, ":warning:", false)} Last updated {schedule.LastUpdated}, may be inaccurate");
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(embed)));
    }
}
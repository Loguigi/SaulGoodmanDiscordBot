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
        // schedule.Update();

        
    }
}
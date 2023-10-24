using System.Globalization;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Library.Helpers;

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
            PictureUrl = picture?.Url
        };
        schedule.WorkSchedule[DayOfWeek.Sunday] = sunday;
        schedule.WorkSchedule[DayOfWeek.Monday] = monday;
        schedule.WorkSchedule[DayOfWeek.Tuesday] = tuesday;
        schedule.WorkSchedule[DayOfWeek.Wednesday] = wednesday;
        schedule.WorkSchedule[DayOfWeek.Thursday] = thursday;
        schedule.WorkSchedule[DayOfWeek.Friday] = friday;
        schedule.WorkSchedule[DayOfWeek.Saturday] = saturday;

        schedule.Update();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("This is what your schedule looks like:").AddEmbed(DisplaySchedule(schedule, ctx.Client))).AsEphemeral());
    }

    [SlashCommand("check", "Check your own or somebody else's schedule")]
    public async Task CheckSchedule(InteractionContext ctx,
        [Option("user", "Schedule of specific user")] DiscordUser? user=null) {
        if (user != null && user.IsBot) {
            await ctx.CreateResponseAsync(StandardOutput.Error("Joe Biden"), ephemeral:true);
            return;
        }

        var schedule = new Schedule(ctx.Guild, user ?? ctx.User);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().AddEmbed(DisplaySchedule(schedule, ctx.Client))));
    }

    [SlashCommand("edit", "Edit a specific day or upload a new schedule picture")]
    public async Task EditSchedule(InteractionContext ctx,
        [Choice("Sunday", (long)DayOfWeek.Sunday)]
        [Choice("Monday", (long)DayOfWeek.Monday)]
        [Choice("Tuesday", (long)DayOfWeek.Tuesday)]
        [Choice("Wednesday", (long)DayOfWeek.Wednesday)]
        [Choice("Thursday", (long)DayOfWeek.Thursday)]
        [Choice("Friday", (long)DayOfWeek.Friday)]
        [Choice("Saturday", (long)DayOfWeek.Saturday)]
        [Option("day", "Day of the week to edit")] long day,
        [Option("newschedule", "Change to the schedule")] string newSchedule) {

        var schedule = new Schedule(ctx.Guild, ctx.User);
        schedule.WorkSchedule[(DayOfWeek)day] = newSchedule;
        schedule.Update();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("Updated schedule:").AddEmbed(DisplaySchedule(schedule, ctx.Client))).AsEphemeral());
    }

    [SlashCommand("picture", "Update your schedule picture")]
    public async Task EditPicture(InteractionContext ctx,
        [Option("newpicture", "New picture")] DiscordAttachment img) {
        
        var schedule = new Schedule(ctx.Guild, ctx.User) {PictureUrl = img.Url};
        schedule.Update();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("Updated schedule:").AddEmbed(DisplaySchedule(schedule, ctx.Client))).AsEphemeral());
    }

    [SlashCommand("today", "List the people that work today")]
    public async Task TodaysSchedules(InteractionContext ctx) {
        try
        {
            var schedules = new List<Schedule>();
            foreach (var user in ctx.Guild.Members) {
                if (!user.Value.IsBot) {
                    schedules.Add(new Schedule(ctx.Guild, user.Value));
                }
            }
            var interactivity = new InteractivityHelper<Schedule>(ctx.Client, schedules.Where(x => x.WorkSchedule[DateTime.Now.DayOfWeek] != null).ToList(), IDHelper.Schedules.Today, "1", "## Nobody works today");

            var embed = new DiscordEmbedBuilder()
                .WithTitle(DateTime.Now.ToString("dddd MMMM d, yyyy"))
                .WithDescription(interactivity.IsEmpty())
                .WithColor(DiscordColor.DarkBlue)
                .WithFooter($"Page {interactivity.Page} of {interactivity.PageLimit}");

            foreach (var schedule in interactivity.GetPage()) {
                embed.Description += $"### {schedule.User.Mention}: {schedule.WorkSchedule[DateTime.Now.DayOfWeek]}\n";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(
                interactivity.AddPageButtons().AddEmbed(embed)));

            ctx.Client.ComponentInteractionCreated -= ScheduleHandler.HandleTodaysSchedules;
            ctx.Client.ComponentInteractionCreated += ScheduleHandler.HandleTodaysSchedules;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    [SlashCommand("override", "Override another user's schedule")]
    [SlashCommandPermissions(Permissions.Administrator)]
    public async Task OverrideSchedule(InteractionContext ctx,
        [Option("user", "Users' schedule to change")] DiscordUser user,
        [Choice("Sunday", (long)DayOfWeek.Sunday)]
        [Choice("Monday", (long)DayOfWeek.Monday)]
        [Choice("Tuesday", (long)DayOfWeek.Tuesday)]
        [Choice("Wednesday", (long)DayOfWeek.Wednesday)]
        [Choice("Thursday", (long)DayOfWeek.Thursday)]
        [Choice("Friday", (long)DayOfWeek.Friday)]
        [Choice("Saturday", (long)DayOfWeek.Saturday)]
        [Option("day", "Day of the week to edit")] long day,
        [Option("newschedule", "Change to the schedule")] string newSchedule) {
        
        if (user.IsBot) {
            await ctx.CreateResponseAsync(StandardOutput.Error("Our programming does not allow the changing of our schedules"), ephemeral:true);
            return;
        }

        var schedule = new Schedule(ctx.Guild, user);
        schedule.WorkSchedule[(DayOfWeek)day] = newSchedule;
        schedule.Update();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("Updated schedule:").AddEmbed(DisplaySchedule(schedule, ctx.Client))).AsEphemeral());
    }

    [SlashCommand("clear", "Clear your schedule for the week")]
    public async Task ClearSchedule(InteractionContext ctx) {
        var schedule = new Schedule(ctx.Guild, ctx.User);
        schedule.Clear();
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("Cleared schedule").AddEmbed(DisplaySchedule(schedule, ctx.Client))).AsEphemeral());
    }

    private DiscordEmbedBuilder DisplaySchedule(Schedule schedule, DiscordClient client) {
        var embed = new DiscordEmbedBuilder()
            .WithAuthor(schedule.User.GlobalName, "", schedule.User.AvatarUrl)
            .WithTitle("Work Schedule")
            .WithDescription(schedule.RecurringSchedule ? "Schedule does not change" : "Schedule changes weekly")
            .WithImageUrl(schedule.PictureUrl ?? "")
            .WithFooter($"Last updated {(schedule.LastUpdated != schedule.NO_DATE ? schedule.LastUpdated : "never")}")
            .WithColor(DiscordColor.Teal);

        foreach (var day in schedule.WorkSchedule) {
            if (day.Value != null) {
                embed.AddField(DateTimeFormatInfo.CurrentInfo.GetDayName(day.Key), day.Value, true);
            }
        }

        if (!schedule.RecurringSchedule && DateTime.Now > schedule.LastUpdated.AddDays(7)) {
            embed.WithFooter($"{DiscordEmoji.FromName(client, ":warning:", false)} Last updated {(schedule.LastUpdated != schedule.NO_DATE ? schedule.LastUpdated : "never")}, may be inaccurate");
        }

        return embed;
    }
}
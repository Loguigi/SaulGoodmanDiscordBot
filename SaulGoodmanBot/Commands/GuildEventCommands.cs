using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SaulGoodmanBot.Commands;

public class GuildEventCommands : ApplicationCommandModule 
{
    [SlashCommand("event", "Create a Discord event")]
    public async Task CreateEvent(InteractionContext ctx,
        [Option("name", "Name of event")] string name,
        [Choice("IRL", 1)]
        [Choice("Voice chat", 2)]
        [Option("type", "Type of event")] long type,
        [Option("start_date", "Start date of event")] string startDate,
        [Option("start_time", "Start time of event")] string startTime,
        [Option("description", "Description of event")] string description="",
        [Option("irl_location", "Location of IRL event")] string irlLocation="TDB",
        [Option("voice_chat", "Voice chat for voice event")][ChannelTypes(ChannelType.Voice)] DiscordChannel? channel=null,
        [Option("mentions", "Users/roles to mention for event. Can be multiple")] string mentions="") 
    {
        
        if (!DateTime.TryParse(startDate, out DateTime date)) 
        {
            throw new Exception("Invalid start date");
        }

        if (!TimeOnly.TryParse(startTime, out TimeOnly time)) 
        {
            throw new Exception("Invalid start time");
        }

        var start = date.AddHours(time.Hour).AddMinutes(time.Minute);

        if (type == 1) 
        {
            await ctx.Guild.CreateEventAsync(
                name, description, null, ScheduledGuildEventType.External, 
                ScheduledGuildEventPrivacyLevel.GuildOnly, start, start.AddHours(12), irlLocation);
        } 
        else 
        {
            var defaultVoice = ctx.Guild.Channels.First(x => x.Value.Type == ChannelType.Voice).Value;
            await ctx.Guild.CreateEventAsync(
                name, description, channel?.Id ?? defaultVoice.Id, ScheduledGuildEventType.VoiceChannel, 
                ScheduledGuildEventPrivacyLevel.GuildOnly, start, null, irlLocation);
        }

        var embed = new DiscordEmbedBuilder()
            .WithAuthor(ctx.User.GlobalName, "", ctx.User.AvatarUrl)
            .WithTitle("New event created")
            .WithColor(DiscordColor.Blue);

        await ctx.CreateResponseAsync(embed);
    }
}
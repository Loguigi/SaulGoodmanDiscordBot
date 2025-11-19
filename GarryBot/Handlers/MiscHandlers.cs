using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class MiscHandlers(
    ServerMemberManager memberManager,
    ServerConfigManager configManager,
    Random random,
    ILogger<MiscHandlers> logger)
    : IEventHandler<ComponentInteractionCreatedEventArgs>, IEventHandler<VoiceStateUpdatedEventArgs>
{
    private readonly List<string> _eggs =
    [
        "https://c.tenor.com/PQvNYusuOI8AAAAd/tenor.gif",
        "https://c.tenor.com/J49gBTuj-SYAAAAC/tenor.gif",
        "https://c.tenor.com/vfzIDhQwkBAAAAAd/tenor.gif",
        "https://c.tenor.com/25m_-RtJ78YAAAAd/tenor.gif",
        "https://c.tenor.com/5-smcmfumnsAAAAC/tenor.gif",
        "https://c.tenor.com/sdn8FD9xZDEAAAAC/tenor.gif",
        "https://c.tenor.com/9X0oPE0NH-oAAAAC/tenor.gif",
        "https://media.tenor.com/l_QovZ2fRm8AAAAi/eg-babei-egg.gif",
        "https://c.tenor.com/BLnW_FXfGGoAAAAC/tenor.gif",
        "https://c.tenor.com/LRT9-BMh938AAAAd/tenor.gif"
    ];
    
    public Task HandleEventAsync(DiscordClient s, ComponentInteractionCreatedEventArgs e)
    {
        var id = IDHelper.GetId(e.Id, 0);

        logger.LogDebug("Component interaction received: {ComponentId} from {User}", id, e.User.Username);

        return id switch
        {
            IDHelper.Misc.EGG => HandleEggCounter(e),
            IDHelper.Misc.WHO => HandleIdentityList(e),
            _ => Task.CompletedTask
        };
    }

    public async Task HandleEventAsync(DiscordClient s, VoiceStateUpdatedEventArgs e)
    {
        // Check if user left a voice channel
        if (e.Before.ChannelId != null && e.After.ChannelId != e.Before.ChannelId)
        {
            var channelLeftFrom = await e.Before.GetChannelAsync();
            
            if (channelLeftFrom!.Users.Count == 1)
            {
                var remainingMember = channelLeftFrom.Users[0];
                var memberLeft = await e.Before.GetUserAsync();
                var guild = await e.GetGuildAsync();
                
                logger.LogDebug("{User} left voice channel {Channel}, {RemainingUser} is now alone", 
                    memberLeft!.Username, channelLeftFrom.Name, remainingMember.Username);

                var member = await memberManager.GetMember(remainingMember, guild!);
                member.EggCount++;
                await memberManager.UpdateMemberAsync(member);

                var config = await configManager.GetConfig(guild!);
                var defaultChannel = config.DefaultChannel ?? guild!.GetDefaultChannel();
                
                var egg = _eggs[random.Next(0, _eggs.Count)];

                await defaultChannel!.SendMessageAsync(MessageTemplates.CreateEggNotification(member, egg));
            }
        }
    }

    private async Task HandleEggCounter(ComponentInteractionCreatedEventArgs e)
    {
        try
        {
            logger.LogDebug("Handling egg counter for user {User}", e.User.Username);
            
            var page = IDHelper.GetId(e.Id, 1);
            var members = await memberManager.GetMembersAsync(e.Guild);
            
            await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateEggCounter(e.Guild, members, page)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling egg counter for user {User}", e.User.Username);
            throw;
        }
    }

    private async Task HandleIdentityList(ComponentInteractionCreatedEventArgs e)
    {
        try
        {
            logger.LogDebug("Handling identity page turn for user {User}", e.User.Username);
            
            var page = IDHelper.GetId(e.Id, 1);
            var members = await memberManager.GetMembersAsync(e.Guild);
            
            await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder(MessageTemplates.CreateIdentityDisplay(e.Guild, members, page)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling identity page turn for user {User}", e.User.Username);
            throw;
        }
    }
}
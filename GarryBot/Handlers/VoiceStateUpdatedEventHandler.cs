using DSharpPlus;
using DSharpPlus.EventArgs;
using GarryLibrary.Helpers;
using GarryLibrary.Managers;
using GarryLibrary.Models;
using Microsoft.Extensions.Logging;

namespace GarryBot.Handlers;

public class VoiceStateUpdatedEventHandler(
    ServerMemberManager memberManager, 
    ServerConfigManager configManager,
    Random random,
    ILogger<VoiceStateUpdatedEventHandler> logger) 
    : IEventHandler<VoiceStateUpdatedEventArgs>
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
        "https://c.tenor.com/LRT9-BMh938AAAAd/tenor.gif",
        "https://c.tenor.com/BFneuY3HqyAAAAAd/tenor.gif",
    ];
    
    public async Task HandleEventAsync(DiscordClient s, VoiceStateUpdatedEventArgs e)
    {
        await HandleRottenEgg(e);
    }

    private async Task HandleRottenEgg(VoiceStateUpdatedEventArgs e)
    {
        ServerMember member;
        
        try
        {
            if (e.Before is null) return;
            
            // Check if user left a voice channel
            if (e.Before.ChannelId != null && e.After.ChannelId != e.Before.ChannelId)
            {
                var channelLeftFrom = await e.Before.GetChannelAsync();

                if (channelLeftFrom!.Users.Count == 1)
                {
                    var remainingMember = channelLeftFrom.Users[0];
                    var memberLeft = await e.Before.GetUserAsync();
                    var guild = await e.GetGuildAsync();

                    if (remainingMember.IsBot)
                    {
                        member = await memberManager.GetMember(memberLeft!, guild!);
                        member.EggCount += 3;
                    }
                    else
                    {
                        member = await memberManager.GetMember(remainingMember, guild!);
                        member.EggCount++;
                    }
                    await memberManager.UpdateMemberAsync(member);

                    var config = await configManager.GetConfig(guild!);
                    var defaultChannel = config.DefaultChannel ?? guild!.GetDefaultChannel();

                    var egg = _eggs[random.Next(0, _eggs.Count)];

                    await defaultChannel!.SendMessageAsync(MessageTemplates.CreateEggNotification(member, egg));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling rotten egg");
            throw;
        }
    }
}
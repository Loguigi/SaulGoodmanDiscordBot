using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SaulGoodmanBot.Commands;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RequireLoguigiAttribute : CheckBaseAttribute {
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) 
    {
        return Task.FromResult(ctx.User.Id == ulong.Parse(Environment.GetEnvironmentVariable("LOGUIGI") ?? throw new Exception("Loguigi UID not set")));
    }
}

[RequireLoguigi]
public class SecretCommands : BaseCommandModule 
{
    #region Bot Control
    [Command("status")]
    public async Task SetBotStatus(CommandContext ctx, string appear, string status) 
    {
        await ctx.Message.DeleteAsync();
        await ctx.Client.UpdateStatusAsync
        (
            activity: new DiscordActivity(status, ActivityType.Playing),
            userStatus: appear switch 
            {
                "on" => UserStatus.Online,
                "idle" => UserStatus.Idle,
                "dnd" => UserStatus.DoNotDisturb,
                "invis" => UserStatus.Invisible,
                _ => UserStatus.Offline
        });
    }

    [Command("leave")]
    public async Task ForceLeaveServer(CommandContext ctx) 
    {
        await ctx.Message.DeleteAsync();
        await ctx.Guild.LeaveAsync();
    }
    #endregion
}
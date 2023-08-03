using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using SaulGoodmanBot.Library;
using SaulGoodmanBot.Handlers;

namespace SaulGoodmanBot.Commands;

[GuildOnly]
[SlashCommandGroup("role", "temp description")]
public class RoleCommands : ApplicationCommandModule {
    [SlashCommand("setup", "temp description")]
    [SlashCommandPermissions(Permissions.Administrator)]
    public async Task Setup(InteractionContext ctx) {
        // var roleList = new ServerRoles(ctx.Guild);
        try {
            var botPosition = ctx.Guild.Roles.Values
                .Where(x => x.Name == "Saul Goodman").First().Position;
            var sortedRoles = ctx.Guild.Roles.Values
                .Where(x => x.Position < botPosition && x.Position != 0 && x.Name != "Nitro Booster")
                .OrderByDescending(x => x.Position).ToList();
            var roleOptions = new List<DiscordSelectComponentOption>();
            foreach (var role in sortedRoles) {
                roleOptions.Add(new DiscordSelectComponentOption(role.Name, role.Id.ToString(), "", false));
            }
            var dropdown = new DiscordRoleSelectComponent("rolesetup", "Select a role");

            var message = new DiscordMessageBuilder()
                .WithContent("test")
                .AddComponents(dropdown);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message));
        } catch (DSharpPlus.Exceptions.BadRequestException e) {
            Console.WriteLine(e.Message);
        }
    }
}

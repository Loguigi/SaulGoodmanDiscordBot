using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

public class ReactionCommands : ApplicationCommandModule {
    [SlashCommand("r", "Send a custom reaction")]
    public async Task CustomReaction(InteractionContext cmd,
        [ChoiceProvider(typeof(ReactionChoiceProvider))][Option("reaction", "funny picture")] string pic) {
        var response = new DiscordEmbedBuilder()
            .WithImageUrl(pic);
        await cmd.CreateResponseAsync(response);
    }
}

public class ReactionChoiceProvider : IChoiceProvider {
    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() {
        var choices = new List<DiscordApplicationCommandOptionChoice>();
        foreach (var reaction in ImageHelper.Reactions) {
            choices.Add(new DiscordApplicationCommandOptionChoice(reaction.Key, reaction.Value));
        }
        await Task.CompletedTask;
        return choices;
    }
}
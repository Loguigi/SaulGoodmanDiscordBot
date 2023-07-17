using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SaulGoodmanBot.Commands;

public class TextCommands : BaseCommandModule {
    [Command("flip")]
    public async Task CoinFlip(CommandContext cmd) {
        Random coin = new Random();
        int flip = coin.Next(1, 3);
        await cmd.Channel.SendMessageAsync((flip == 1) ? $"`Heads` {cmd.Message.Author.Mention}" : $"`Tails` {cmd.Message.Author.Mention}");
    }

    [Command("wheel")]
    public async Task Wheel(CommandContext cmd, List<String> contents) {
        if (contents.Count == 0) {
            await cmd.Channel.SendMessageAsync("`Error: theres nothing to put in the wheel you god damn idiot`");
        }
        else {
            // TODO
        }
    }
}
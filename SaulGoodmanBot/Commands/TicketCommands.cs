using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("ticket", "Commands to create feature idea and issue tickets for the bot")]
public class TicketCommands : ApplicationCommandModule {
    [SlashCommand("create", "Create a new ticket")]
    public async Task CreateTicket(InteractionContext ctx) {
        try {
            
        } catch (Exception ex) {

        }
    }

    [SlashCommand("list", "List all open tickets")]
    public async Task ListTickets(InteractionContext ctx) {
        try {
            
        } catch (Exception ex) {

        }
    }
}
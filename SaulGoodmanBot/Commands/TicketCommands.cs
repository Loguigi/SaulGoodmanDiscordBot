using System.Reflection;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SaulGoodmanBot.Controllers;
using SaulGoodmanBot.Library;

namespace SaulGoodmanBot.Commands;

[SlashCommandGroup("ticket", "Commands to create feature idea and issue tickets for the bot")]
public class TicketCommands : ApplicationCommandModule {
    [SlashCommand("create", "Create a new ticket")]
    public async Task CreateTicket(InteractionContext ctx) {
        try {
            await Task.CompletedTask;
            throw new NotImplementedException();
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    [SlashCommand("list", "List all open tickets")]
    public async Task ListTickets(InteractionContext ctx) {
        try {
            await Task.CompletedTask;
            throw new NotImplementedException();
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
}
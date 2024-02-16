using SaulGoodmanBot.Library;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using Dapper;
using DSharpPlus;
using DSharpPlus.Entities;

namespace SaulGoodmanBot.Controllers;

public class TicketMaster : DbBase {
    private DiscordClient Client { get; set; }
    public List<Ticket> Tickets { get; set; } = new();

    public TicketMaster(DiscordClient client) {
        try {
            Client = client;
            var tickets = GetData<TicketModel>("", new DynamicParameters()).Result;
            Tickets = MapData(tickets.Result!);
        } catch (Exception ex) {
            throw;
        }
    }

    public void OpenTicket(Ticket ticket) {
        try {
            var result = SaveData("", new DynamicParameters()); // todo
        } catch (Exception ex) {

        }
    }

    public void CloseTicket(Ticket ticket) {

    }

    public void AddAction(Ticket ticket, TicketAction action) {

    }

    public void AddVote(Ticket ticket, DiscordUser user) {
        try {
            var result = SaveData("", new DynamicParameters(
                new TicketVoteModel() {
                    TicketId = ticket.Id,
                    UserId = (long)user.Id
            })).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {

        }
    }

    private List<Ticket> MapData(List<TicketModel> data)
    {
        var tickets = new List<Ticket>();
        foreach (var t in data) {
            var ticket = new Ticket();
            var actions = GetData<TicketActionModel>("", new DynamicParameters(new { TicketId = ticket.Id })).Result;
            var votes = GetData<TicketVoteModel>("", new DynamicParameters(new { TicketId = ticket.Id })).Result;

            foreach (var v in votes.Result!) {
                ticket.Votes.Add(GetUser(Client, (ulong)v.UserId).Result);
            }

            foreach (var a in actions.Result!) {
                ticket.Actions.Add(new TicketAction() { Action = a.Action, ActionTime = a.ActionTime });
            }

            tickets.Add(ticket);
        }

        return tickets;
    }
}
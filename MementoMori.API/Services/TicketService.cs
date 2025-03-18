using MementoMori.API.Data;
using MementoMori.API.Entities;
using MementoMori.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MementoMori.API.Services;

public class TicketService(AppDbContext context) : ITicketService
{
    public async Task<TicketDto?> GetTicketAsync(Guid id)
    {
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null)
            return null;

        return ticket.ToDto();
    }

    public async Task<TicketDto> CreateTicketAsync(Guid userId, TicketForCreationDto ticketForCreationDto)
    {
        var ticket = new Ticket
        {
            UserId = userId,
            Title = ticketForCreationDto.Title,
            TicketType = Constants.TicketType.Active,
            TicketComments =
            [
                new()
                {
                    UserId = userId,
                    Value = ticketForCreationDto.Description
                }
            ]
        };

        await context.Tickets.AddAsync(ticket);
        await context.SaveChangesAsync();

        return ticket.ToDto();
    }

    public async Task AddCommentAsync(Guid userId, Guid ticketId, string comment)
    {
        var ticketComment = new TicketComment
        {
            UserId = userId,
            Value = comment,
            TicketId = ticketId
        };

        await context.TicketComments.AddAsync(ticketComment);
        await context.SaveChangesAsync();
    }
}
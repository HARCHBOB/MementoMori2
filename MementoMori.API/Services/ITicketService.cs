using MementoMori.API.Entities;
using MementoMori.API.Models;

namespace MementoMori.API.Services;

public interface ITicketService
{
    Task<TicketDto?> GetTicketAsync(Guid id);
    Task<TicketDto> CreateTicketAsync(Guid userId, TicketForCreationDto ticketForCreationDto);
    Task AddCommentAsync(Guid userId, Guid ticketId, string comment);
}
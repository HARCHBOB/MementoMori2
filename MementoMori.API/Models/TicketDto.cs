using MementoMori.API.Constants;

namespace MementoMori.API.Models;

public class TicketDto
{
    public Guid Id { get; set; }
    
    public required Guid UserId { get; set; }

    public required TicketType TicketType { get; set; }

    public required string Title { get; set; } = string.Empty;

    public List<TicketCommentDto> TicketComments { get; set; } = [];
}
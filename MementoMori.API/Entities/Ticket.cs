using MementoMori.API.Constants;
using MementoMori.API.Models;

namespace MementoMori.API.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    
    public required Guid UserId { get; set; }

    public required TicketType TicketType { get; set; }

    public required string Title { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public List<TicketComment> TicketComments { get; set; } = [];

    public TicketDto ToDto()
    {
        return new TicketDto()
        {
            Id = Id,
            UserId = UserId,
            TicketType = TicketType,
            Title = Title,
            TicketComments = 
            [
                .. TicketComments.Select(tc => new TicketCommentDto
                    {
                        Id = tc.Id,
                        UserId = tc.UserId,
                        Value = tc.Value
                    }
                )
            ]
        };
    }
}
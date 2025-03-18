using System.ComponentModel.DataAnnotations;

namespace MementoMori.API.Entities;

public class TicketComment
{
    [Key]
    public Guid Id { get; set; }

    public Guid TicketId { get; set; }

    public Guid UserId { get; set; }

    public string Value { get; set; } = string.Empty;

    public Ticket Ticket { get; set; } = null!;

    public User User { get; set; } = null!;
}
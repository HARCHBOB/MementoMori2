using System.ComponentModel.DataAnnotations;

namespace MementoMori.API.Models;

public class TicketCommentDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Value { get; set; } = string.Empty;
}
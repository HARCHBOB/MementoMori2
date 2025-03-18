using MementoMori.API.Models;
using MementoMori.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MementoMori.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TicketController(IAuthService authService, ITicketService ticketService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
    {
        var userId = authService.GetRequesterId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var ticket = await ticketService.GetTicketAsync(id);

        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] TicketForCreationDto ticketForCreationDto)
    {
        var userId = authService.GetRequesterId(HttpContext);
        if (userId == null)
            return Unauthorized();

        var ticket = await ticketService.CreateTicketAsync(userId.Value, ticketForCreationDto);

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPost("{id}/comment")]
    public async Task<ActionResult> AddComment(Guid id, [FromBody] TicketCommentDto ticketCommentDto)
    {
        var userId = authService.GetRequesterId(HttpContext);
        if (userId == null)
            return Unauthorized();

        await ticketService.AddCommentAsync(userId.Value, id, ticketCommentDto.Value);

        return Ok();
    }
}
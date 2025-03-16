using MementoMori.API.DTOS;
using MementoMori.API.Interfaces;
using MementoMori.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MementoMori.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserDecksController(IDeckHelper deckHelper, IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IDeckHelper _deckHelper = deckHelper;

    [HttpGet("userInformation")]
    public async Task<ActionResult> UserInformation()
    {
        var requesterId = _authService.GetRequesterId(HttpContext);
        if (requesterId != null)
        {
            var userDecks = await _deckHelper.GetUserDecks((Guid)requesterId);
            var userInfo = new UserDeckInformationDTO{
                Decks = userDecks ?? [],
                IsLoggedIn = true,
            };

            return Ok(userInfo);
        }
        else
        {
            var userDecks = new UserDeckInformationDTO{Decks = null, IsLoggedIn = false};
            return Ok(userDecks);
        }

    }
    [HttpGet("userCollectionDecksController")]
    public async Task<ActionResult> UserCollectionDecksController()
    {
        var requesterId = _authService.GetRequesterId(HttpContext);
        if (requesterId != null)
        {
            var userDecks = await _deckHelper.GetUserCollectionDecks((Guid)requesterId);
            var userInfo = new UserDeckInformationDTO
            {
                Decks = userDecks ?? [],
                IsLoggedIn = true,
            };
            return Ok(userInfo);
        }
        else
        {
            var userDecks = new UserDeckInformationDTO{Decks = null, IsLoggedIn = false};
            return Ok(userDecks);
        }
    }

    [HttpPost("userCollectionRemoveDeckController")]
    public async Task<ActionResult> UserCollectionRemoveDeckController(DatabaseObject deckId)
    {
        var requesterId = _authService.GetRequesterId(HttpContext);
        if (deckId.Id == Guid.Empty)
            return StatusCode(400);
        if (requesterId != null)
        {
            await _deckHelper.DeleteUserCollectionDeck(deckId.Id, (Guid) requesterId);
            return Ok();
        }
        else
        {
            return Unauthorized();
        }
    }

}
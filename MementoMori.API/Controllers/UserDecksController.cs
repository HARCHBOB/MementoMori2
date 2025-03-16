using MementoMori.API.Models;
using MementoMori.API.Services;
using MementoMori.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MementoMori.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserDecksController(IDeckHelper deckHelper, IAuthService authService) : ControllerBase
{
    [HttpGet("userInformation")]
    public ActionResult UserInformation()
    {
        var requesterId = authService.GetRequesterId(HttpContext);
        if (requesterId != null)
        {
            var userDecks = deckHelper.GetUserDecks((Guid)requesterId);
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
    public ActionResult UserCollectionDecksController()
    {
        var requesterId = authService.GetRequesterId(HttpContext);
        if (requesterId != null)
        {
            var userDecks = deckHelper.GetUserCollectionDecks((Guid)requesterId);
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
    public ActionResult UserCollectionRemoveDeckController(DatabaseObject deckId)
    {
        var requesterId = authService.GetRequesterId(HttpContext);
        if (deckId.Id == Guid.Empty)
            return StatusCode(400);

        if (requesterId != null)
        {
            deckHelper.DeleteUserCollectionDeck(deckId.Id, (Guid) requesterId);
            return Ok();
        }
        else
        {
            return Unauthorized();
        }
    }

}
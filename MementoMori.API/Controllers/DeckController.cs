using Microsoft.AspNetCore.Mvc;
using MementoMori.API.Models;
using MementoMori.API.Extensions;
using MementoMori.API.Services;
using MementoMori.API.Exceptions;
using MementoMori.API.Constants;

namespace MementoMori.API.Controllers;

[ApiController]
[Route("[controller]/{deckId}")]
public class DecksController(IDeckHelper deckHelper, IAuthService authService, ICardService cardService, IAuthRepo authRepo) : ControllerBase
{
    [HttpGet("deck")]
    public ActionResult ViewAsync(Guid deckId)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { errorCode = ErrorCode.InvalidInput, message = "Invalid deck ID." });

        var requesterId = authService.GetRequesterId(HttpContext);

        var deck = deckHelper.Filter(ids: [deckId], userId: requesterId).FirstOrDefault();

        if (deck == null)
            return NotFound("Deck not found.");

        var DeckDTO = new DeckDTO
        {
            Id = deck.Id,
            CreatorName = deck.Creator?.Username ?? "deleted",
            CardCount = deck.Cards.Count,
            Modified = deck.Modified,
            Rating = deck.Rating,
            Tags = deck.TagsToString(),
            Title = deck.Title,
            Description = deck.Description,
            IsOwner = requesterId != null && requesterId == deck.Creator?.Id,
            InCollection = deckHelper.IsDeckInCollection(deckId, requesterId)
        };
        return Ok(DeckDTO);
    }

    [HttpGet("EditorView")]
    public ActionResult EditorViewAsync(Guid deckId)
    {
        if (deckId == Guid.Empty)
            return BadRequest(new { errorCode = ErrorCode.InvalidInput, message = "Invalid deck ID." });

        var requesterId = authService.GetRequesterId(HttpContext);

        var deck = deckHelper.Filter(ids: [deckId], userId: requesterId).FirstOrDefault();

        if (deck == null)
            return NotFound("Deck not found.");

        var DTO = new DeckEditorDTO
        {
            Id = deck.Id,
            isPublic = deck.IsPublic,
            CardCount = deck.Cards.Count,
            Description = deck.Description,
            Tags = deck.TagsToString(),
            Title = deck.Title,
            Cards = [
                .. deck.Cards
                    .Select(Card => new CardDTO
                        {
                            Id = Card.Id,
                            Question = Card.Question,
                            Description = Card.Description,
                            Answer = Card.Answer,

                        }
                    )
                ]
        };

        return Ok(DTO);
    }

    [HttpGet("cards")]
    public async Task<ActionResult> GetDueCards(Guid deckId)
    {
        var userId = authService.GetRequesterId(HttpContext);

        if (deckId == Guid.Empty || userId == null)
            return BadRequest(new { errorCode = "InvalidInput", message = "Invalid deck or user ID." });

        var user = await authRepo.GetUserByIdAsync(userId.Value);
        if (user == null)
            return BadRequest(new { errorCode = "InvalidInput", message = "Invalid deck or user ID." });

        var dueForReviewCards = cardService.GetCardsForReview(deckId, userId.Value);
        if (dueForReviewCards.Count == 0)
            return NotFound("No cards due for review.");

        var dueCardDtos = dueForReviewCards
            .Select(c => 
                new CardDTO
                {
                    Id = c.Id,
                    Question = c.Question,
                    Description = c.Description,
                    Answer = c.Answer
                }
            )
            .ToList();

        return Ok(new { Cards = dueCardDtos, Color = user.CardColor });
    }
    [HttpPost("addToCollection")]
    public IActionResult AddCardsToCollection(Guid deckId)
    {
        var userId = authService.GetRequesterId(HttpContext);

        if (deckId == Guid.Empty || userId == Guid.Empty || userId == null)
            return BadRequest(new { errorCode = "InvalidInput", message = "Invalid deck or user ID." });

        if (userId != null)
            cardService.AddCardsToCollection(userId.Value, deckId);

        return Ok();
    }

    [HttpPost("cards/update/{cardId}")]
    public async Task<IActionResult> UpdateCard(Guid deckId, Guid cardId, [FromBody] int quality)
    {
        var userId = authService.GetRequesterId(HttpContext);

        if (deckId == Guid.Empty || userId == null || cardId == Guid.Empty)
            return BadRequest(new { errorCode = "InvalidInput", message = "Invalid deck, card, or user ID." });

        try
        {
            await cardService.UpdateSpacedRepetition(userId.Value, deckId, cardId, quality);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { errorCode = "NotFound", message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating card: {ex.Message}\n{ex.StackTrace}");

            return StatusCode(500);
        }
    }
    [HttpPost("editDeck")]
    public async Task<ActionResult> EditDeck(EditedDeckDTO editedDeckDTO)
    {
        var requesterId = authService.GetRequesterId(HttpContext);
        if (requesterId == null)
            return Unauthorized();

        try
        {
            await deckHelper.UpdateDeckAsync(editedDeckDTO, (Guid)requesterId);
        }
        catch (UnauthorizedEditingException)
        {
            return Unauthorized();
        }

        return Ok();
    }

    [HttpPost("createDeck")]
    public async Task<ActionResult<Guid>> CreateDeck(EditedDeckDTO createDeckDTO)
    {
        var requesterId = authService.GetRequesterId(HttpContext);
        if (requesterId == null)
            return Unauthorized();
        
        try
        {
            var newDeckId = await deckHelper.CreateDeckAsync(createDeckDTO, requesterId.Value);
            return Ok(newDeckId);
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("deleteDeck")]
    public async Task<ActionResult> DeleteDeck(Guid deckId)
    {
        var requesterId = authService.GetRequesterId(HttpContext);
        try
        {
            if (requesterId != null)
            {
                await deckHelper.DeleteDeckAsync(deckId, requesterId.Value);
                return Ok();
            }
            else
                return Unauthorized();
        }
        catch
        {
            return StatusCode(500);
        }

    }
    [HttpGet("DeckTitle")]
    public IActionResult GetDeckTitle(Guid deckId)
    {
        var deck = deckHelper.GetDeckAsync(deckId);

        if (deck == null)
            return NotFound(new { errorCode = ErrorCode.NotFound, message = "Deck not found." });

        return Ok(deck.Title);
    }
}
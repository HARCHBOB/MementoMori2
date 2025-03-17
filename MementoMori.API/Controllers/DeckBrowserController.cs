using MementoMori.API.Models;
using MementoMori.API.Extensions;
using MementoMori.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MementoMori.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DeckBrowserController(IDeckHelper deckHelper, IAuthService authService) : ControllerBase
{
    private readonly IDeckHelper _deckHelper = deckHelper;
    private readonly IAuthService _authService = authService;

    [HttpGet("getDecks")]
    public ActionResult<DeckBrowserDTO> GetDecksAsync([FromQuery] string[] selectedTags, string? searchString)
    {
        var requesterId = _authService.GetRequesterId(HttpContext);

        var filteredDecksList = _deckHelper.Filter(titleSubstring: searchString, selectedTags: selectedTags, userId: requesterId);
        filteredDecksList.Sort();

        var result = filteredDecksList
            .Select(deck => new DeckBrowserDTO
            {
                Id = deck.Id,
                Title = deck.Title,
                Rating = deck.Rating,
                Modified = deck.Modified,
                Cards = deck.Cards.Count,
                Tags = deck.TagsToString(),
            })
            .ToArray();

        return Ok(result);
    }
}
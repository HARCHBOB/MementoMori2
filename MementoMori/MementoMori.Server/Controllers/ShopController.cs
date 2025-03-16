using MementoMori.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MementoMori.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopController(IAuthService authService, IAuthRepo authRepo) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IAuthRepo _authRepo = authRepo;

    [HttpPost("newColor")]
    public async Task<IActionResult> UpdateCardColor([FromBody] UpdateColorRequest request)
    {
        var userId = _authService.GetRequesterId(HttpContext);
        if (userId == null)
        {
            return Unauthorized();
        }

        await _authRepo.UpdateUserCardColor((Guid)userId, request.NewColor);

        return Ok();
    }
}
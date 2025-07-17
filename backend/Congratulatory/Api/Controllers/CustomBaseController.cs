using CoreService.Result;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoreService.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomControllerBase : ControllerBase
{
    protected Task<IActionResult> HandleResult<T>(ServiceResult<T> result)
    {
        if (result.Success)
        {
            return Task.FromResult<IActionResult>(StatusCode(result.StatusCode, ApiResult.Ok(result.Data)));
        }

        return Task.FromResult<IActionResult>(StatusCode(result.StatusCode, 
            ApiResult.Fail(result.ErrorMessage!, result.Errors)));
    }

    protected Guid? UserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
}
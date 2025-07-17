using CoreService.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace CoreService.Api.Filters;

public class ValidateTokenAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedObjectResult(ApiResult.Fail("Token is missing or invalid"));
            return;
        }

        var token = authorizationHeader.Replace("Bearer ", "");

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                context.Result = new UnauthorizedObjectResult(ApiResult.Fail("Invalid token format"));
                return;
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out _))
            {
                context.Result = new UnauthorizedObjectResult(ApiResult.Fail("Invalid token: UserId is missing or invalid"));
                return;
            }
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedObjectResult(ApiResult.Fail("Error while validating token"));
        }
    }
}
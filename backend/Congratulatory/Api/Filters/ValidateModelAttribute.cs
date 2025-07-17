using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CoreService.Result;

namespace CoreService.Api.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.ToDictionary(
                kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                kvp => string.Join("; ", kvp.Value.Errors.Select(e => e.ErrorMessage))
            );

            var response = ApiResult.Fail("Ошибки валидации", errors.ToDictionary(
                kvp => kvp.Key,
                kvp => new[] { kvp.Value }
            ));

            context.Result = new BadRequestObjectResult(response);
        }
    }
}
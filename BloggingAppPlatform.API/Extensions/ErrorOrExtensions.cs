using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BloggingAppPlatform.API.Extensions;

public abstract class ApiController : ControllerBase
{
    protected IActionResult ErrorResult(List<Error> errors)
    {
        if (errors.Count == 0)
            return Problem();

        var first = errors[0];

        var statusCode = first.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            statusCode: statusCode,
            title: first.Code,
            detail: first.Description);
    }
}

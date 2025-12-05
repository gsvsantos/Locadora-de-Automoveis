using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebAPI.Extensions;

public static class ApiResponseExtensions
{
    public static IActionResult ToHttpResponse<TResponse>(this Result<TResponse> result)
    {
        if (result.IsFailed)
        {
            string[] messages = result.Errors
                .SelectMany(e => e.Reasons.OfType<IError>())
                .Select(e => e.Message)
                .ToArray();

            return new JsonResult(messages)
            {
                StatusCode = MapErrorStatusCode(result)
            };
        }

        return new JsonResult(result.Value)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public static IActionResult ToHttpResponse(this Result result)
    {
        if (result.IsFailed)
        {
            string[] messages = result.Errors
                .SelectMany(e => e.Reasons.OfType<IError>())
                .Select(e => e.Message)
                .ToArray();

            return new JsonResult(messages)
            {
                StatusCode = MapErrorStatusCode(result)
            };
        }

        return new JsonResult(null)
        {
            StatusCode = StatusCodes.Status204NoContent
        };
    }

    private static int MapErrorStatusCode(ResultBase result)
    {
        string? type = result.Errors
            .SelectMany(e => e.Metadata)
            .Where(kvp => kvp.Key == "ErrorType")
            .Select(kvp => kvp.Value?.ToString())
            .FirstOrDefault();

        return type switch
        {
            // 409 (Conflict)
            "Conflict" => StatusCodes.Status409Conflict,
            "DuplicateRecord" => StatusCodes.Status409Conflict,
            "DeletionBlocked" => StatusCodes.Status409Conflict,

            // 404 (Not Found)
            "NotFound" => StatusCodes.Status404NotFound,
            "RecordNotFound" => StatusCodes.Status404NotFound,

            // 400 (Bad Request)
            "BadRequest" => StatusCodes.Status400BadRequest,

            // 401 / 403
            "Unauthorized" => StatusCodes.Status401Unauthorized,
            "Forbidden" => StatusCodes.Status403Forbidden,

            // 500
            "InternalServer" => StatusCodes.Status500InternalServerError,

            // fallback
            _ => StatusCodes.Status400BadRequest
        };
    }
}

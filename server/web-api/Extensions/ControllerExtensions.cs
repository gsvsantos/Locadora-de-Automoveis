using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace LocadoraDeAutomoveis.WebAPI.Extensions;

public static class ControllerExtensions
{
    public static ActionResult MapFailure(this ControllerBase controller, Result result)
    {
        string[] messages = result.Errors
            .SelectMany(e => e.Reasons.OfType<IError>())
            .Select(e => e.Message)
            .ToArray();

        bool hasErrorType = result.Errors.Any(e => e.HasMetadataKey("ErrorType"));

        if (!hasErrorType)
        {
            return controller.StatusCode(StatusCodes.Status500InternalServerError);
        }

        string? type = result.Errors
            .SelectMany(e => e.Metadata)
            .Where(kvp => kvp.Key == "ErrorType")
            .Select(kvp => kvp.Value?.ToString())
            .FirstOrDefault();

        return type switch
        {
            // 409 (Conflict)
            "Conflict" => controller.Conflict(messages),   // 409
            "DuplicateRecord" => controller.Conflict(messages),  // 409
            "DeletionBlocked" => controller.Conflict(messages),  // 409

            // 404 (Not Found)
            "NotFound" => controller.NotFound(messages),  // 404
            "RecordNotFound" => controller.NotFound(messages),  // 404

            // 400 (Bad Request)
            "BadRequest" => controller.BadRequest(messages),   // 400

            // 401/403 (Auth)
            "Unauthorized" => controller.Unauthorized(),   // 401
            "Forbidden" => controller.Forbid(),  // 403

            // 500 (Internal Server Error)
            "InternalServer" => controller.StatusCode(500, messages),  // 500

            // fallback
            _ => controller.BadRequest(messages)   // fallback
        };
    }
}

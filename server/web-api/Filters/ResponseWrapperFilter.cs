using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LocadoraDeAutomoveis.WebApi.Filters;

public class ResponseWrapperFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is JsonResult jsonResult)
        {
            object? value = jsonResult.Value;

            if (value is IEnumerable<string> errorMessages)
            {
                jsonResult.Value = new
                {
                    Success = false,
                    Errors = errorMessages
                };
            }
            else
            {
                jsonResult.Value = new
                {
                    Success = true,
                    Data = value
                };
            }
        }
    }
}
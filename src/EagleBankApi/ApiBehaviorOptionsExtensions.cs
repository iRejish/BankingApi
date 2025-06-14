using EagleBankApi.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EagleBankApi;

internal static class ApiBehaviorOptionsExtensions
{
    public static readonly Func<ActionContext, IActionResult> ApiStandardErrorFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.ValidationState == ModelValidationState.Invalid)
            .SelectMany(entry => entry.Value!.Errors
                .Select(error => new BadRequestErrorResponse.ErrorDetail
                {
                    Field = entry.Key,
                    Message = error.ErrorMessage,
                    Type = "ModelValidationFailure"
                }))
            .ToList();

        return new BadRequestObjectResult(new BadRequestErrorResponse
        {
            Message = "Error: Bad Request",
            Details = errors
        });
    };
}

using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ApiResponseControllerExtensions
{
    public static IActionResult ApiOk(this ControllerBase _, object? data = null, int statusCode = StatusCodes.Status200OK) =>
        new ObjectResult(ApiResponse.Ok(data, statusCode)) { StatusCode = statusCode };

    public static IActionResult ApiBadRequest(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse.Fail(message, StatusCodes.Status400BadRequest))
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

    public static IActionResult ApiNotFound(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse.Fail(message, StatusCodes.Status404NotFound))
        {
            StatusCode = StatusCodes.Status404NotFound
        };

    public static IActionResult ApiConflict(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse.Fail(message, StatusCodes.Status409Conflict))
        {
            StatusCode = StatusCodes.Status409Conflict
        };

    public static IActionResult ApiUnauthorized(this ControllerBase _, string message) =>
        new ObjectResult(ApiResponse.Fail(message, StatusCodes.Status401Unauthorized))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

    public static IActionResult ApiValidationProblem(this ControllerBase controller)
    {
        if (controller.ModelState.IsValid)
            return controller.ApiOk();

        var msgs = string.Join(
            "; ",
            controller.ModelState.Values.SelectMany(v => v.Errors).Select(e =>
                string.IsNullOrEmpty(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage));

        return controller.ApiBadRequest(string.IsNullOrEmpty(msgs) ? "Validation failed." : msgs);
    }
}

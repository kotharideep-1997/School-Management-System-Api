using Api.Extensions;
using Application.DTO;
using Application.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PermissionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>Set permission for user: insert row or update IsActive if pair already exists.</summary>
    [HttpPost("set")]
    public async Task<IActionResult> SetPermission([FromBody] SetUserPermissionRequestDto request)
    {
        if (request is null || request.UserId <= 0 || request.PermissionMasterId <= 0)
            return this.ApiBadRequest("userId and permissionMasterId are required.");

        await _unitOfWork.Permissions.SetUserPermissionAsync(
            request.UserId,
            request.PermissionMasterId,
            request.IsActive);

        return this.ApiOk(new { Message = "Permission set successfully." });
    }

    /// <summary>Update IsActive only for an existing user–permission row.</summary>
    [HttpPut("active")]
    public async Task<IActionResult> UpdateActive([FromBody] UpdateUserPermissionActiveRequestDto request)
    {
        if (request is null || request.UserId <= 0 || request.PermissionMasterId <= 0)
            return this.ApiBadRequest("userId and permissionMasterId are required.");

        var updated = await _unitOfWork.Permissions.UpdateUserPermissionActiveAsync(
            request.UserId,
            request.PermissionMasterId,
            request.IsActive);

        if (!updated)
        {
            return this.ApiNotFound(
                "No user permission row found for this userId and permissionMasterId.");
        }

        return this.ApiOk(new { Message = "Permission active flag updated." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rows = await _unitOfWork.Permissions.GetUserPermissionsAsync();
        return this.ApiOk(rows);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        if (userId <= 0)
            return this.ApiBadRequest("Invalid user id.");

        var rows = await _unitOfWork.Permissions.GetUserPermissionsByUserIdAsync(userId);
        return this.ApiOk(rows);
    }
}

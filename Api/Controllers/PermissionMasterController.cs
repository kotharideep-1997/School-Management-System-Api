using Api.Extensions;
using Application.DTO;
using Application.IUnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PermissionMasterController : ControllerBase
{
    private const int MaxPermissionNameLength = 50;

    private readonly IUnitOfWork _unitOfWork;

    public PermissionMasterController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private static PermissionMasterResponseDto ToResponse(PermissionMaster p) => new()
    {
        Id = p.Id,
        PermissionName = p.PermissionName,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        IsActive = p.IsActive
    };

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionMasterRequestDto request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.PermissionName))
            return this.ApiBadRequest("Invalid data");

        var name = request.PermissionName.Trim();
        if (name.Length > MaxPermissionNameLength)
            return this.ApiBadRequest($"Permission name must be at most {MaxPermissionNameLength} characters.");

        var now = DateTime.UtcNow;
        var entity = new PermissionMaster
        {
            PermissionName = name,
            IsActive = request.IsActive,
            CreatedAt = now,
            UpdatedAt = now
        };

        var id = await _unitOfWork.PermissionMasters.AddAsync(entity);
        return this.ApiOk(new { Id = id, Message = "Permission created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.PermissionMasters.GetAllAsync();
        return this.ApiOk(data.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var permission = await _unitOfWork.PermissionMasters.GetByIdAsync(id);
        if (permission is null)
            return this.ApiNotFound("Permission not found");

        return this.ApiOk(ToResponse(permission));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePermissionMasterRequestDto request)
    {
        if (request is null || request.Id == 0 || string.IsNullOrWhiteSpace(request.PermissionName))
            return this.ApiBadRequest("Invalid data");

        var name = request.PermissionName.Trim();
        if (name.Length > MaxPermissionNameLength)
            return this.ApiBadRequest($"Permission name must be at most {MaxPermissionNameLength} characters.");

        var existing = await _unitOfWork.PermissionMasters.GetByIdAsync(request.Id);
        if (existing is null)
            return this.ApiNotFound("Permission not found");

        var now = DateTime.UtcNow;
        var entity = new PermissionMaster
        {
            Id = request.Id,
            PermissionName = name,
            IsActive = request.IsActive,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = now
        };

        var result = await _unitOfWork.PermissionMasters.UpdateAsync(entity);
        if (!result)
            return this.ApiNotFound("Permission not found");

        return this.ApiOk(new { Message = "Updated successfully" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _unitOfWork.PermissionMasters.GetByIdAsync(id);
        if (existing is null)
            return this.ApiNotFound("No permission exists with this id. Delete was not performed.");

        var deleted = await _unitOfWork.PermissionMasters.DeleteAsync(id);
        if (!deleted)
            return this.ApiNotFound("Permission could not be deleted.");

        return this.ApiOk(new { Message = "Deleted successfully" });
    }
}

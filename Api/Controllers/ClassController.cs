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
public class ClassController : ControllerBase
{
    private const int MaxClassLabelLength = 5;

    private readonly IUnitOfWork _unitOfWork;

    public ClassController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private static ClassMasterResponseDto ToResponse(ClassMaster c) => new()
    {
        Id = c.Id,
        Class = c.Class,
        Strength = c.Strength,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        IsActive = c.IsActive
    };

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassRequestDto request)
    {
        if (request is null)
            return this.ApiBadRequest("Invalid data.");
        if (!ModelState.IsValid)
            return this.ApiValidationProblem();
        if (string.IsNullOrWhiteSpace(request.Class))
            return this.ApiBadRequest("class cannot be empty or whitespace.");

        var label = request.Class.Trim();
        if (label.Length > MaxClassLabelLength)
        {
            return this.ApiBadRequest(
                $"Class label must be at most {MaxClassLabelLength} characters (e.g. 5A, 12B, 10A).");
        }

        var now = DateTime.UtcNow;
        var entity = new ClassMaster
        {
            Class = label,
            Strength = request.Strength,
            IsActive = request.IsActive,
            CreatedAt = now,
            UpdatedAt = now
        };

        var id = await _unitOfWork.Classes.AddAsync(entity);
        return this.ApiOk(new { Id = id, Message = "Class created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.Classes.GetAllAsync();
        return this.ApiOk(data.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cls = await _unitOfWork.Classes.GetByIdAsync(id);
        if (cls is null)
            return this.ApiNotFound("Class not found");

        return this.ApiOk(ToResponse(cls));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateClassRequestDto request)
    {
        if (request is null || request.Id == 0)
            return this.ApiBadRequest("Invalid data.");
        if (!ModelState.IsValid)
            return this.ApiValidationProblem();
        if (string.IsNullOrWhiteSpace(request.Class))
            return this.ApiBadRequest("class cannot be empty or whitespace.");

        var label = request.Class.Trim();
        if (label.Length > MaxClassLabelLength)
        {
            return this.ApiBadRequest(
                $"Class label must be at most {MaxClassLabelLength} characters (e.g. 5A, 12B, 10A).");
        }

        var existing = await _unitOfWork.Classes.GetByIdAsync(request.Id);
        if (existing is null)
            return this.ApiNotFound("Class not found");

        var now = DateTime.UtcNow;
        var entity = new ClassMaster
        {
            Id = request.Id,
            Class = label,
            Strength = request.Strength,
            IsActive = request.IsActive,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = now
        };

        var result = await _unitOfWork.Classes.UpdateAsync(entity);
        if (!result)
            return this.ApiNotFound("Class not found");

        return this.ApiOk(new { Message = "Updated successfully" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _unitOfWork.Classes.GetByIdAsync(id);
        if (existing is null)
            return this.ApiNotFound("No class exists with this id. Delete was not performed.");

        var deleted = await _unitOfWork.Classes.DeleteAsync(id);
        if (!deleted)
            return this.ApiNotFound("Class could not be deleted.");

        return this.ApiOk(new { Message = "Deleted successfully" });
    }
}

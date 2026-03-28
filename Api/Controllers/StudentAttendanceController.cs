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
public class StudentAttendanceController : ControllerBase
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Present",
        "Absent"
    };

    private readonly IUnitOfWork _unitOfWork;

    public StudentAttendanceController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private static StudentAttendanceResponseDto ToResponse(StudentAttendance a) => new()
    {
        Id = a.Id,
        StudentId = a.StudentId,
        AttendanceDate = a.AttendanceDate,
        Status = a.Status,
        Created_At = a.Created_At,
        Updated_At = a.Updated_At
    };

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentAttendanceRequestDto request)
    {
        if (request is null)
            return this.ApiBadRequest("Invalid data.");
        if (request.StudentId <= 0)
            return this.ApiBadRequest("StudentId is required.");
        if (string.IsNullOrWhiteSpace(request.Status) || !AllowedStatuses.Contains(request.Status.Trim()))
            return this.ApiBadRequest("Status must be Present or Absent.");

        var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
        if (student is null)
            return this.ApiNotFound("Student not found.");

        var entity = new StudentAttendance
        {
            StudentId = request.StudentId,
            AttendanceDate = request.AttendanceDate,
            Status = request.Status.Trim()
        };

        var id = await _unitOfWork.StudentAttendances.AddAsync(entity);
        return this.ApiOk(new { Id = id, Message = "Attendance saved successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.StudentAttendances.GetAllAsync();
        return this.ApiOk(data.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var row = await _unitOfWork.StudentAttendances.GetByIdAsync(id);
        if (row is null)
            return this.ApiNotFound("Attendance record not found.");

        return this.ApiOk(ToResponse(row));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStudentAttendanceRequestDto request)
    {
        if (request is null || request.Id <= 0)
            return this.ApiBadRequest("Invalid data.");
        if (request.StudentId <= 0)
            return this.ApiBadRequest("StudentId is required.");
        if (string.IsNullOrWhiteSpace(request.Status) || !AllowedStatuses.Contains(request.Status.Trim()))
            return this.ApiBadRequest("Status must be Present or Absent.");

        var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
        if (student is null)
            return this.ApiNotFound("Student not found.");

        var existing = await _unitOfWork.StudentAttendances.GetByIdAsync(request.Id);
        if (existing is null)
            return this.ApiNotFound("Attendance record not found.");

        var entity = new StudentAttendance
        {
            Id = request.Id,
            StudentId = request.StudentId,
            AttendanceDate = request.AttendanceDate,
            Status = request.Status.Trim(),
            Created_At = existing.Created_At,
            Updated_At = existing.Updated_At
        };

        var ok = await _unitOfWork.StudentAttendances.UpdateAsync(entity);
        if (!ok)
            return this.ApiNotFound("Attendance record not found.");

        return this.ApiOk(new { Message = "Updated successfully" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _unitOfWork.StudentAttendances.GetByIdAsync(id);
        if (existing is null)
            return this.ApiNotFound("No attendance record exists with this id. Delete was not performed.");

        var deleted = await _unitOfWork.StudentAttendances.DeleteAsync(id);
        if (!deleted)
            return this.ApiNotFound("Attendance record could not be deleted.");

        return this.ApiOk(new { Message = "Deleted successfully" });
    }
}

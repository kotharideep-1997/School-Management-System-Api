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
public class StudentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudent dto)
    {
        if (dto == null)
            return this.ApiBadRequest("Invalid data.");

        if (!ModelState.IsValid)
            return this.ApiValidationProblem();

        var classRow = await _unitOfWork.Classes.GetByIdAsync(dto.ClassId);
        if (classRow is null)
        {
            return this.ApiBadRequest(
                "No class found for the given classId. Use GET api/Class to list valid ids.");
        }

        if (dto.Active && classRow.Strength > 0)
        {
            var enrolled = await _unitOfWork.Students.CountActiveStudentsInClassAsync(dto.ClassId);
            if (enrolled >= classRow.Strength)
            {
                return this.ApiBadRequest(
                    $"This class has reached its maximum strength of {classRow.Strength} active students. Cannot add another student.");
            }
        }

        if (await _unitOfWork.Students.ExistsRollNoInClassAsync(dto.ClassId, dto.RollNo))
        {
            return this.ApiBadRequest(
                "A student with this roll number already exists in this class. Please use another roll number.");
        }

        var student = new Student
        {
            ClassId = dto.ClassId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            RollNo = dto.RollNo,
            Active = dto.Active
        };
        var id = await _unitOfWork.Students.AddAsync(student);

        return this.ApiOk(new { Id = id, Message = "Student created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.Students.GetAllAsync();
        return this.ApiOk(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);

        if (student == null)
            return this.ApiNotFound("Student not found");

        return this.ApiOk(student);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateStudentDto dto)
    {
        if (dto == null)
            return this.ApiBadRequest("Invalid data");

        if (!ModelState.IsValid)
            return this.ApiValidationProblem();

        var existing = await _unitOfWork.Students.GetByIdAsync(dto.Id);
        if (existing is null)
            return this.ApiNotFound("Student not found");

        var classRow = await _unitOfWork.Classes.GetByIdAsync(dto.ClassId);
        if (classRow is null)
        {
            return this.ApiBadRequest(
                "No class found for the given classId. Use GET api/Class to list valid ids.");
        }

        if (dto.Active && classRow.Strength > 0)
        {
            var enrolled = await _unitOfWork.Students.CountActiveStudentsInClassAsync(dto.ClassId, dto.Id);
            if (enrolled >= classRow.Strength)
            {
                return this.ApiBadRequest(
                    $"This class has reached its maximum strength of {classRow.Strength} active students. Cannot activate or move the student into this class.");
            }
        }

        if (await _unitOfWork.Students.ExistsRollNoInClassAsync(dto.ClassId, dto.RollNo, dto.Id))
        {
            return this.ApiBadRequest(
                "A student with this roll number already exists in this class. Please use another roll number.");
        }

        var model = new Student
        {
            Id = dto.Id,
            ClassId = dto.ClassId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            RollNo = dto.RollNo,
            Active = dto.Active,
            CreatedDate = existing.CreatedDate,
            UpdatedDate = existing.UpdatedDate
        };

        var result = await _unitOfWork.Students.UpdateAsync(model);

        if (!result)
            return this.ApiNotFound("Student not found");

        return this.ApiOk(new { Message = "Updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _unitOfWork.Students.GetByIdAsync(id);
        if (existing is null)
            return this.ApiNotFound("No student exists with this id. Delete was not performed.");

        var deleted = await _unitOfWork.Students.DeleteAsync(id);
        if (!deleted)
            return this.ApiNotFound("Student could not be deleted.");

        return this.ApiOk(new { Message = "Deleted successfully" });
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(StudentSearchRequestDto request)
    {
        var data = await _unitOfWork.Students.SearchAsync(request);
        return this.ApiOk(data);
    }

    [HttpPost("paged")]
    public async Task<IActionResult> GetPaged(PagedRequestDto request)
    {
        var data = await _unitOfWork.Students.GetPagedAsync(request);
        return this.ApiOk(data);
    }
}

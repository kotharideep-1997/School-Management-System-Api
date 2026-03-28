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
    public async Task<IActionResult> Update(Student model)
    {
        if (model == null || model.Id == 0)
            return this.ApiBadRequest("Invalid data");

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

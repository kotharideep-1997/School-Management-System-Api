using Api.Extensions;
using Application.DTO;
using Application.Helpers;
using Application.IUnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UserController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrEmpty(dto.Password))
            return this.ApiBadRequest("Invalid data");

        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
        var user = new User
        {
            UserName = dto.UserName.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedDate = DateTime.UtcNow,
            IsActive = dto.IsActive
        };

        var id = await _unitOfWork.Users.AddAsync(user);
        return this.ApiOk(new { Id = id, Message = "User created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var result = users.Select(u => new { u.Id, u.UserName, u.CreatedDate, u.IsActive });
        return this.ApiOk(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
            return this.ApiNotFound("User not found");

        return this.ApiOk(new { user.Id, user.UserName, user.CreatedDate, user.IsActive });
    }

    [HttpGet("by-name/{userName}")]
    public async Task<IActionResult> GetByUserName(string userName)
    {
        var user = await _unitOfWork.Users.GetByUserNameAsync(userName);
        if (user == null)
            return this.ApiNotFound("User not found");

        return this.ApiOk(new { user.Id, user.UserName, user.CreatedDate, user.IsActive });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest dto)
    {
        if (dto == null || dto.Id == 0 || string.IsNullOrWhiteSpace(dto.UserName))
            return this.ApiBadRequest("Invalid data");

        var existing = await _unitOfWork.Users.GetByIdAsync(dto.Id);
        if (existing == null)
            return this.ApiNotFound("User not found");

        existing.UserName = dto.UserName.Trim();
        existing.IsActive = dto.IsActive;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
            existing.PasswordHash = hash;
            existing.PasswordSalt = salt;
        }

        var ok = await _unitOfWork.Users.UpdateAsync(existing);
        if (!ok)
            return this.ApiNotFound("User not found");

        return this.ApiOk(new { Message = "Updated successfully" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _unitOfWork.Users.GetByIdAsync(id);
        if (existing is null)
            return this.ApiNotFound("No user exists with this id. Delete was not performed.");

        var deleted = await _unitOfWork.Users.DeleteAsync(id);
        if (!deleted)
            return this.ApiNotFound("User could not be deleted.");

        return this.ApiOk(new { Message = "Deleted successfully" });
    }
}

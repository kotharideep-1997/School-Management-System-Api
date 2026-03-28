using Api.Extensions;
using Application.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AttendanceReportController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceReportController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetForStudent(int studentId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _unitOfWork.AttendanceReports.GetAttendanceForStudentAsync(studentId, from, to);
        return this.ApiOk(data);
    }

    [HttpGet("date")]
    public async Task<IActionResult> GetForDate([FromQuery] DateTime date)
    {
        var data = await _unitOfWork.AttendanceReports.GetAttendanceForDateAsync(date);
        return this.ApiOk(data);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummaryForDate([FromQuery] DateTime date)
    {
        var data = await _unitOfWork.AttendanceReports.GetAttendanceSummaryForDateAsync(date);
        return this.ApiOk(data);
    }
}

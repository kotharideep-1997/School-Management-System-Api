using Api.Extensions;
using Application.DTO;
using Application.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("active-students-count")]
    public async Task<IActionResult> GetActiveStudentCount()
    {
        var count = await _unitOfWork.Reports.GetActiveStudentCountAsync();
        return this.ApiOk(count);
    }

    [HttpGet("active-users-count")]
    public async Task<IActionResult> GetActiveUserCount()
    {
        var count = await _unitOfWork.Reports.GetActiveUserCountAsync();
        return this.ApiOk(count);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var students = await _unitOfWork.Reports.GetActiveStudentCountAsync();
        var users = await _unitOfWork.Reports.GetActiveUserCountAsync();
        return this.ApiOk(new { ActiveStudentCount = students, ActiveUserCount = users });
    }

    [HttpGet("attendance-range-summary")]
    public async Task<IActionResult> GetAttendanceRangeSummary([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
            return this.ApiBadRequest("fromDate must be on or before toDate.");

        var summary = await _unitOfWork.Reports.GetAttendanceRangeSummaryAsync(fromDate, toDate);
        return this.ApiOk(summary);
    }

    [HttpPost("attendance/present")]
    public async Task<IActionResult> GetPresentStudentsForRange([FromBody] AttendanceReportFilterDto filter)
    {
        if (filter.FromDate.Date > filter.ToDate.Date)
            return this.ApiBadRequest("FromDate must be on or before ToDate.");

        filter.PageNumber = Math.Max(1, filter.PageNumber);
        filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);

        var result = await _unitOfWork.Reports.GetPresentStudentsForRangeAsync(filter);
        return this.ApiOk(result);
    }

    [HttpPost("attendance/absent")]
    public async Task<IActionResult> GetAbsentStudentsForRange([FromBody] AttendanceReportFilterDto filter)
    {
        if (filter.FromDate.Date > filter.ToDate.Date)
            return this.ApiBadRequest("FromDate must be on or before ToDate.");

        filter.PageNumber = Math.Max(1, filter.PageNumber);
        filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);

        var result = await _unitOfWork.Reports.GetAbsentStudentsForRangeAsync(filter);
        return this.ApiOk(result);
    }
}

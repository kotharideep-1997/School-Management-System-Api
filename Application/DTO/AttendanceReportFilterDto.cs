namespace Application.DTO;

/// <summary>Filters for attendance reports. Date range is required; name and roll are optional.</summary>
public class AttendanceReportFilterDto : PagedRequestDto
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    /// <summary>Optional: matches first name, last name, or full name (contains).</summary>
    public string? Name { get; set; }

    public int? RollNo { get; set; }
}

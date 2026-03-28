namespace Application.DTO;

/// <summary>Distinct students with at least one Present / Absent mark in the date range.</summary>
public class AttendanceRangeSummaryDto
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int PresentStudentCount { get; set; }

    public int AbsentStudentCount { get; set; }
}

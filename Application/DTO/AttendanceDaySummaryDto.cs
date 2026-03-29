namespace Application.DTO;

public class AttendanceDaySummaryDto
{
    public DateTime Date { get; set; }

    public int PresentCount { get; set; }

    public int AbsentCount { get; set; }

    public IReadOnlyList<AttendanceGridRowDto> PresentData { get; set; } = Array.Empty<AttendanceGridRowDto>();

    public IReadOnlyList<AttendanceGridRowDto> AbsentData { get; set; } = Array.Empty<AttendanceGridRowDto>();

    public IReadOnlyList<AttendanceGridRowDto> OtherData { get; set; } = Array.Empty<AttendanceGridRowDto>();
}

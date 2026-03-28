namespace Application.DTO;

/// <summary>Flat attendance row for API responses (no navigation properties).</summary>
public class StudentAttendanceResponseDto
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime Created_At { get; set; }

    public DateTime Updated_At { get; set; }
}

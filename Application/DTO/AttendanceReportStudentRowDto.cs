namespace Application.DTO;

public class AttendanceReportStudentRowDto
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public int RollNo { get; set; }

    public int ClassId { get; set; }

    public string? ClassLabel { get; set; }
}

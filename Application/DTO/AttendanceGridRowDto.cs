namespace Application.DTO;

public class AttendanceGridRowDto
{
    public int RollNo { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Class { get; set; } = string.Empty;

    public DateTime AttendanceDate { get; set; }

    public string Attendance { get; set; } = string.Empty;
}

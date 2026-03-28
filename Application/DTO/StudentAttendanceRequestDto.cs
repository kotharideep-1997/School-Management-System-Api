namespace Application.DTO;

public class CreateStudentAttendanceRequestDto
{
    public int StudentId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class UpdateStudentAttendanceRequestDto
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

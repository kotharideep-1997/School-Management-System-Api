namespace Application.DTO;

public class AttendanceGridFilterDto
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string? StudentName { get; set; }

    public int? RollNo { get; set; }
}

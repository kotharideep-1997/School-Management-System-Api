using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class UpdateStudentDto
{
    [Range(1, int.MaxValue, ErrorMessage = "id is required.")]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "classId must be a valid class id.")]
    public int ClassId { get; set; }

    [Required(ErrorMessage = "firstName is required.")]
    [StringLength(40)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "lastName is required.")]
    [StringLength(40)]
    public string LastName { get; set; } = string.Empty;

    public int RollNo { get; set; }

    public bool Active { get; set; } = true;
}

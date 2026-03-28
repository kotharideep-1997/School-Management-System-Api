using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class CreateStudent
{
    /// <summary>Required. Must match an existing row in ClassMaster.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "classId is required and must be a positive id that exists in ClassMaster.")]
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

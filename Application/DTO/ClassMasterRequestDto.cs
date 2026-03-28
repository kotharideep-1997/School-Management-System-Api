using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class CreateClassRequestDto
{
    /// <summary>Short class label (1–5 chars), e.g. 5A, 12B, 10A. Not the Swagger placeholder "string" (too long).</summary>
    [Required(ErrorMessage = "class is required.")]
    [StringLength(5, MinimumLength = 1, ErrorMessage = "Class label must be between 1 and 5 characters (e.g. 5A, 12B, 10A).")]
    public string Class { get; set; } = string.Empty;

    public byte Strength { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateClassRequestDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "class is required.")]
    [StringLength(5, MinimumLength = 1, ErrorMessage = "Class label must be between 1 and 5 characters (e.g. 5A, 12B, 10A).")]
    public string Class { get; set; } = string.Empty;

    public byte Strength { get; set; }

    public bool IsActive { get; set; }
}

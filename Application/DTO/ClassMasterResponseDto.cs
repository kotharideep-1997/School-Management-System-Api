namespace Application.DTO;

/// <summary>Class row for API responses (no navigation properties).</summary>
public class ClassMasterResponseDto
{
    public int Id { get; set; }

    /// <summary>Short label (max 5 chars), e.g. 5A, 12B.</summary>
    public string Class { get; set; } = string.Empty;

    public byte Strength { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class StudentClassMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ClassId { get; set; }

        public bool IsActive { get; set; }

        [Column("Created_At")]
        public DateTime CreatedAt { get; set; }

        [Column("Updated_At")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        // 🔗 Navigation (Many → One)
        [ForeignKey("ClassId")]
        public ClassMaster Class { get; set; }

    }
}
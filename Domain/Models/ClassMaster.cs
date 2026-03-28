using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Table("ClassMaster")]
    public class ClassMaster
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Short label, e.g. 5A, 12B, 10A (max 5 characters).</summary>
        [Required, StringLength(5)]
        public string Class { get; set; }

        [Required]
        public byte Strength { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
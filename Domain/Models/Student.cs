using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int Id { get; set; }

        // ✅ Foreign Key
        [Required]
        public int ClassId { get; set; }

        [Required, StringLength(40)]
        public string FirstName { get; set; }

        [Required, StringLength(40)]
        public string LastName { get; set; }

        [Required]
        public int RollNo { get; set; }

        public bool Active { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("ClassId")]
        public ClassMaster Class { get; set; }

        public ICollection<StudentAttendance> Attendances { get; set; }

    }
}

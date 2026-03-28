using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [Table("StudentAttendances")]
    public class StudentAttendance
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string Status { get; set; }  // Present / Absent

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}

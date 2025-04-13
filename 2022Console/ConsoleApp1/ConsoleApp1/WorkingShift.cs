using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class WorkingShift
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime TimeBegin { get; set; }
        [Required]
        public DateTime TimeEnd { get; set; }
        [Required]
        public int EmployerId { get; set; }
        public Employer Employer { get; set; }
        public List<Event> Events { get; set; }
    }
}

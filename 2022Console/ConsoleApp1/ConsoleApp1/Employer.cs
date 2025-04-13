using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Employer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Fio { get; set; } = null!;
        [Required]
        public int AreaId { get; set; }
        public Area Area { get; set; }
        public List<WorkingShift> Shifts {  get; set; }
    }
}

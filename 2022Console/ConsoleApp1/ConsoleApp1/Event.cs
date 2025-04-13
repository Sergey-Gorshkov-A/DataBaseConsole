using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConsoleApp1
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DayId { get; set; }
        [Required]
        public Day Day { get; set; }
        public int AnimalId { get; set; }
        public Animal Animal { get; set; } 
        [Required]
        public int WorkingShiftId { get; set; }
        public WorkingShift WorkingShift { get; set; }
        [Required]
        public bool IsBorn { get; set; }
        [Required]
        public bool IsDie { get; set; }
        public string? EventText { get; set; }
        [Required]
        public DateTime EventTime { get; set; }
    }
}

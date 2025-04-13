using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string? Area_name { get; set; }
        [Required]
        public string Adress { get; set; } = null!;
        [Required]
        public List<Aviary> Aviaries { get; set; }
        public List<Employer> Employers { get; set; }
    }
}

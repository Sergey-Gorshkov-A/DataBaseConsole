using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AviaryId { get; set; }
        public Aviary Aviary { get; set; }
        [Required]
        public string TypeOfAnimal { get; set; } = null!;
        [Required]
        public string Status { get; set; } = null!;

        public List<Event> Events { get; set; }

        public AdultAnimal AdultAnimal { get; set; }
        public BabyAnimal BabyAnimal { get; set; }
    }
}

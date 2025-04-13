using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class BabyAnimal
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public int AdultAnimalId { get; set; }
        public AdultAnimal? AdultAnimal { get; set; }
    }
}

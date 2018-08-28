﻿namespace PetClinic.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Animal
    {
        public Animal()
        {
            this.Procedures = new List<Procedure>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Type { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Age { get; set; }

        [Required]
        public string PassportSerialNumber { get; set; }

        [Required]
        public Passport Passport { get; set; }

        public ICollection<Procedure> Procedures { get; set; }
    }
}

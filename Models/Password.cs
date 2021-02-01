﻿using System.ComponentModel.DataAnnotations;

namespace enigma_prime.Data
{
    public class Password : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name="Password")]
        public string Pass { get; set; }
        [Required]
        [Display(Name = "Application Type")]
        public string Type { get; set; }
        public string Url { get; set; }
        public string Developer { get; set; }
    }
}
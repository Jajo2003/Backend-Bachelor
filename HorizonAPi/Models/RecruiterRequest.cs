using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HorizonAPi.Models
{
    public class RecruiterRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        public string CompanyName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [MinLength(6)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ContactPerson { get; set; }

    }
}
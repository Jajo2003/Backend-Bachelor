using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HorizonAPi.Models
{
    public class User
    {
        public int Id { get; set; }

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

        [Required]
        public string PasswordHash { get; set; }

        public string? University { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserRole {get; set;}

        public UserDetails? UserDetails { get; set; }

    }

    public class UserDetails
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal ExpectedSalary {get;set;}
        public string Experience { get; set; }
        public string Skills {get;set;}
    }
    public class UpdateUserDetailsRequest
    {
        public decimal ExpectedSalary { get; set; }
        public string Experience { get; set; }
        public string Skills { get; set; }
    }
}
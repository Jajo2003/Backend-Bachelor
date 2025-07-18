using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorizonAPi.Models
{
    public class RecruiterModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }
        
        public string PasswordHash { get; set; }

        public string ContactPerson { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserRole { get; set; }
    
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HorizonAPi.Models
{
    public class LoginRequest
    {
    public string Email { get; set; }

    public string Password { get; set; }

    }
}
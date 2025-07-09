using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HorizonAPi.Models
{
    public class JobRequest
    {   
        public string Title { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string Salary { get; set; }
        public string Location { get; set; }
        public string QualificationLevel { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorizonAPi.Models;

namespace HorizonAPi.DTO
{
    public class JobDto
    {
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public decimal? Salary { get; set; }

    public string QualifyLevel {get;set;}

    public string CompanyName {get;set;}

    public string PostedDateFormatted { get; set; }
    }
}
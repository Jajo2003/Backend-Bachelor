using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorizonAPi.Models
{
    public class Job
{
    public int JobId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Location { get; set; }
    
    public decimal? Salary { get; set; }  
    
    public int PostedBy { get; set; }  
    
    public DateTime PostedDate { get; set; } = DateTime.Now; 
    
    public string Status { get; set; } = "Pending";

    public string QualifyLevel {get;set;}

    public string CompanyName {get;set;}
}
}
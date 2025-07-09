using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HorizonAPi.Models;



public class StudHorizondbContext : DbContext
{
    public DbSet<User> Users { get; set;}
    public DbSet<Job> Jobs {get;set;}
    public DbSet<UserDetails> UserDetails {get;set;}
    public DbSet<RecruiterModel> Recruiters { get; set; }
    public StudHorizondbContext(DbContextOptions<StudHorizondbContext> options) 
    : base(options)
    {

    }
    
    
}

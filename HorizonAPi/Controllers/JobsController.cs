using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HorizonAPi.DTO;
using HorizonAPi.Mappers;
using HorizonAPi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorizonAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly StudHorizondbContext _context;

        public JobsController(StudHorizondbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _context.Jobs.Where(j => j.Status == "Approved").ToListAsync();
            return Ok(jobs);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPost("JobAdd")]
        public async Task<IActionResult> AddJob([FromBody] JobRequest request)
        {
            var recruiterIdClaim = User.Claims.FirstOrDefault(x => x.Type == "Id");
            if (recruiterIdClaim == null)
                return Unauthorized("ERROR OCCURED");
            var recruiterId = int.Parse(recruiterIdClaim.Value);

            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                Salary = Convert.ToDecimal(request.Salary),
                PostedBy = recruiterId,
                PostedDate = DateTime.UtcNow,
                QualifyLevel = request.QualificationLevel,
                Status = "Pending",
                CompanyName = request.CompanyName,
            };

            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Job Requested Successfully"
            });
        }
        [HttpGet("PendingJobs")]
        public async Task<IActionResult> GetPendingJobs()
        {
            var jobs = await _context.Jobs.Where(j => j.Status == "Pending").ToListAsync();
            return Ok(jobs);
        }
        [HttpPut("ApproveJob/{id}")]
        public async Task<IActionResult> ApproveJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found");

            job.Status = "Approved";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Job approved successfully" });
        }

        [HttpDelete("DenyJob/{id}")]
        public async Task<IActionResult> DenyJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Job permanently deleted (denied)" });
        }


    }
}
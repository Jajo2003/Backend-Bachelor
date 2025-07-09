using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using HorizonAPi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace HorizonAPi.Controllers
{
    [Route("api/auth")]
    public class AuthorizationController : Controller
    {
        private readonly StudHorizondbContext _context;
        private readonly IConfiguration _config;

        public AuthorizationController(StudHorizondbContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _context.Users.ToListAsync();
            return Ok(res);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserDetails)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid Credentials" });

            var hashedInput = HashPassword(request.Password);
            if (user.PasswordHash != hashedInput)
                return Unauthorized(new { message = "Invalid Credentials" });

            string generatedToken = TokenGeneration(user);

            return Ok(new
            {
                token = generatedToken,
                message = "Login successful",
                id = user.Id,
                firstname = user.FirstName,
                lastName = user.LastName,
                mail = user.Email,
                Role = user.UserRole,
                expectedSalary = user.UserDetails?.ExpectedSalary,
                experience = user.UserDetails?.Experience,
                skills = user.UserDetails?.Skills
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] RegisterRequest request)
        {
            if(!ValidMail(request.Email))
                return Conflict(new { message = "Invalid Email" });
            var checkExists = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (checkExists != null)
                return Conflict(new { message = "Email is in use" });
            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                University = request.University,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UserRole = "Student"
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var userDetails = new UserDetails
            {
                UserId = newUser.Id,
                ExpectedSalary = 0,
                Experience = "",
                Skills = ""
            };

            await _context.UserDetails.AddAsync(userDetails);
            await _context.SaveChangesAsync();
            string generatedToken = TokenGeneration(newUser);
            return Ok(new
            {
                token = generatedToken,
                message = "Account Created Successfuly",
                id = newUser.Id,
                firstname = newUser.FirstName,
                lastName = newUser.LastName,
                mail = newUser.Email,
                Role = newUser.UserRole,
            });
        }
          [Authorize]
        [HttpPut("update-details")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserDetailsRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.UserId == userId);
            
            if (userDetails == null)
                return NotFound(new { message = "User details not found." });

            userDetails.ExpectedSalary = request.ExpectedSalary;
            userDetails.Experience= request.Experience;
            userDetails.Skills = request.Skills;

            await _context.SaveChangesAsync();

            return Ok(new
                        {
                            message = "User details updated successfully.",
                            Updatedsalary = userDetails.ExpectedSalary,
                            UpdatedExperience = userDetails.Experience,
                            UpdatedSkills = userDetails.Skills
                        });
        }
        [HttpPost("RecruiterLogin")]
        public async Task<IActionResult> RecruiterLogin([FromBody] RecruiterRequest request)
        {
            var user = await _context.Recruiters.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid Credentials" });

            var hashedInput = HashPassword(request.Password);
            if (user.PasswordHash != hashedInput)
                return Unauthorized(new { message = "Invalid Credentials" });

            string generatedToken = AdminTokenGeneration(user);

            return Ok(new
            {
                token = generatedToken,
                id = user.Id,
                message = "Login successful",
                firstname = user.FirstName,
                lastName = user.LastName,
                mail = user.Email,
                Role = user.UserRole
            });
        }
        [HttpPost("RecruiterRegister")]
        public async Task<IActionResult> RecruiterRegister([FromBody] RecruiterRequest request)
        {
            if(!ValidRecruiters(request.Email))
                return Conflict(new { message = "Invalid Email" });
            var checkExists = await _context.Recruiters.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (checkExists != null)
                return Conflict(new { message = "Email is in use" });
            var newRecruiter = new RecruiterModel
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CompanyName = request.CompanyName,
                PasswordHash = HashPassword(request.Password),
                ContactPerson = $"{request.FirstName} {request.LastName}", 
                CreatedAt = DateTime.UtcNow,
                UserRole = "Recruiter"
            };
            await _context.Recruiters.AddAsync(newRecruiter);
            await _context.SaveChangesAsync();

            string generatedToken = AdminTokenGeneration(newRecruiter);
            return Ok(new
            {
                token = generatedToken,
                message = "Account Created Successfuly",
                id = newRecruiter.Id,
                firstname = newRecruiter.FirstName,
                lastName = newRecruiter.LastName,
                mail = newRecruiter.Email,
                Role = newRecruiter.UserRole
            });
        }

        private bool ValidMail(string mail)
        {
            var requirementVal = _config["allowedUsers:UsersRequirement"];

            return mail.EndsWith(requirementVal, StringComparison.OrdinalIgnoreCase);
        }
        private bool ValidRecruiters(string mail)
        {
            var allowedDomains = _config.GetSection("allowedRecruiters").Get<string[]>();
            return allowedDomains.Any(domain => mail.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
        }
        private string TokenGeneration(User user)
        {
            var claims = new[]
             {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.UserRole)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:AuthTokenKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString.ToString();
        }
        private string AdminTokenGeneration(RecruiterModel rec)
        {
            var claims = new[]
            {
                new Claim("Id", rec.Id.ToString()), 
                new Claim(ClaimTypes.Email, rec.Email),
                new Claim(ClaimTypes.Name, rec.FirstName + " " + rec.LastName),
                new Claim(ClaimTypes.Role, rec.UserRole) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:AuthTokenKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
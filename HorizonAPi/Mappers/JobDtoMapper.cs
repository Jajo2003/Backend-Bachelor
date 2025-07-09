using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorizonAPi.DTO;
using HorizonAPi.Models;

namespace HorizonAPi.Mappers
{
    public static class JobDtoMapper
    {
        public static JobDto ToJobDto(this Job jobModel)
        {
            return new JobDto
            {
                Title = jobModel.Title,
                Description = jobModel.Description,
                Location = jobModel.Location,
                Salary = jobModel.Salary,
                QualifyLevel = jobModel.QualifyLevel,
                CompanyName = jobModel.CompanyName,
                PostedDateFormatted = jobModel.PostedDate.ToString("o")
            };
        }
        
    }
}
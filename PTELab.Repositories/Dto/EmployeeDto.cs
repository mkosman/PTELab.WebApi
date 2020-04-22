using System;
using PTELab.Database.Entities;

namespace PTELab.Repositories.Dto
{
    public class EmployeeDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public JobTitles JobTitle { get; set; }
    }
}
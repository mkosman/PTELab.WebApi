using System;
using System.Text;

namespace PTELab.Repositories.Dto
{
    public class SearchRequest
    {
        public string Keyword { get; set; }
        public DateTime? EmployeeDateOfBirthFrom { get; set; }
        public DateTime? EmployeeDateOfBirthTo { get; set; }
        public string[] EmployeeJobTitles { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace PTELab.Repositories.Dto
{
    public class CompanyDto
    {
        public long Id { get;set;}
        public  string Name { get; set; }
        public  int EstablishmentYear { get; set; }
        public  List<EmployeeDto> Employees { get; set; }
    }
}

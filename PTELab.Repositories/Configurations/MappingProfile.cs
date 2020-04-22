using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using PTELab.Database.Entities;
using PTELab.Repositories.Dto;

namespace PTELab.Repositories.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ReverseMap()
                .ForPath(x=>x.Id, opt => opt.Ignore())
                ;
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.JobTitle, 
                    opt => opt.MapFrom<JobTitlesFromStringResolver>())
                ;
            CreateMap<EmployeeDto, Employee>()
                ;
        }
    }

    public class JobTitlesFromStringResolver : IValueResolver<Employee, EmployeeDto, JobTitles>
    {
        public JobTitles Resolve(Employee source, EmployeeDto destination, JobTitles destMember, ResolutionContext context)
        {
            return Enum.Parse<JobTitles>(source.JobTitle);

        }
    }
}

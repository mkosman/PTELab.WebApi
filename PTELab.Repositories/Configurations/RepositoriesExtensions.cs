using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PTELab.Repositories.CompanyRepos;
using PTELab.Repositories.Dto;

namespace PTELab.Repositories.Configurations
{
   public static class RepositoriesExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICompanyRepo, CompanyRepo>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(CompanyDtoValidator))
                .AddClasses(@class => @class.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces());
            
            return services;
        }
    }
}

using System;
using FluentValidation;
using PTELab.Database.Entities;

namespace PTELab.Repositories.Dto
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            When(x => x.EmployeeDateOfBirthFrom.HasValue, () =>
            {
                RuleFor(x => x.EmployeeDateOfBirthFrom.Value)
                    .Must(x => !(x == DateTime.MinValue) && !(x == DateTime.MaxValue));
            });

            When(x => x.EmployeeDateOfBirthTo.HasValue, () =>
            {
                RuleFor(x => x.EmployeeDateOfBirthTo.Value)
                    .Must(x => !(x == DateTime.MinValue) && !(x == DateTime.MaxValue));
            });

            When(x => x.EmployeeDateOfBirthTo.HasValue && x.EmployeeDateOfBirthFrom.HasValue, () =>
            {
                RuleFor(x => x.EmployeeDateOfBirthFrom)
                    .Must((model, field) => model.EmployeeDateOfBirthTo != null && field != null && (field.Value < model.EmployeeDateOfBirthTo.Value))
                    .WithMessage("Value EmployeeDateOfBirthFrom is greater to EmployeeDateOfBirthTo");
            });

            When(x => x.EmployeeJobTitles != null, () =>
            {
                RuleForEach(x => x.EmployeeJobTitles)
                    .IsEnumName(typeof(JobTitles))
                    ;
            });
        }
    }
}
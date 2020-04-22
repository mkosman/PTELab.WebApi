using System;
using FluentValidation;

namespace PTELab.Repositories.Dto
{
    public class EmployeeDtoValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100)
                ;
            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100)
                ;
            RuleFor(x => x.DateOfBirth)
                .NotNull()
                .GreaterThan(DateTime.MinValue)
                ;
            RuleFor(x => x.JobTitle)
                .IsInEnum()
                ;
        }
    }
}
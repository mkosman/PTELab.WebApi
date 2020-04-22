using FluentValidation;

namespace PTELab.Repositories.Dto
{
    public class CompanyDtoValidator : AbstractValidator<CompanyDto>
    {
        public CompanyDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                ;
            RuleFor(x => x.EstablishmentYear)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Establishment year cannot be  less that 0")
                ;
        }
    }
}
using Domain.DTOs;
using FluentValidation;

namespace Application.Features.Validations
{
    public class DemoValidator : AbstractValidator<DemoDTO>
    {
        public DemoValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("Name is Required")
                .MaximumLength(100);

            RuleFor(m => m.Age)
                .NotEmpty()
                .NotNull()
                .WithMessage("Age is Required")
                .MaximumLength(50);

            RuleFor(m => m.Address)
                .NotEmpty()
                .NotNull()
                .WithMessage("Address is Required")
                .MaximumLength(500);
        }
    }
}

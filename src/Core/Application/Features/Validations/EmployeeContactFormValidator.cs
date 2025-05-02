using Application.DTOs;
using FluentValidation;

namespace Application.Features.Validations
{
    public class EmployeeContactFormValidator : AbstractValidator<EmployeeContactDto>
    {
        public EmployeeContactFormValidator()
        {
            RuleFor(m => m.FullName)
                .NotEmpty()
                .NotNull()
                .WithMessage("Name is Required")
                .Must(BeAString)
                .WithMessage("Name must be text and not literally 'string'")
                .MaximumLength(100);

            RuleFor(m => m.Phone)
                .NotEmpty()
                .NotNull()
                .WithMessage("Phone number is Required")
                .Must(BeAString)
                .WithMessage("Phone number must be text and not literally 'string'")
                .MaximumLength(50);

            RuleFor(m => m.Email)
                .NotEmpty()
                .NotNull()
                .WithMessage("Email Address is Required")
                .Must(BeAString)
                .WithMessage("Email must be text and not literally 'string'")
                .MaximumLength(500)
                .EmailAddress()
                .WithMessage("Please provide a valid email address");

            RuleFor(m => m.LinkedIn)
                .NotEmpty()
                .NotNull()
                .WithMessage("LinkedIn is required")
                .Must(BeAString)
                .WithMessage("LinkedIn must be text and not literally 'string'")
                .MaximumLength(500);
        }

        private bool BeAString(object value)
        {
            // Check if the value is actually a string and not literally the word "string"
            return value is string && value.ToString().Trim().ToLower() != "string";
        }
    }
}

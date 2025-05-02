using Application.DTOs;
using Application.Features.Defaults;
using FluentValidation;

namespace Application.Features.Validations;

public class EmployeeContactHandler
{
    private readonly IValidator<EmployeeContactDto> _validator;
    private readonly EmployeeContactDtoDefaults _defaults;

    public EmployeeContactHandler(
        IValidator<EmployeeContactDto> validator,
        EmployeeContactDtoDefaults defaults)
    {
        _validator = validator;
        _defaults = defaults;
    }

    public async Task Handle(EmployeeContactDto dto)
    {
        // Apply defaults from appsettings
        _defaults.ApplyDefaults(dto);

        // Validate
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Proceed with other logic
    }
}
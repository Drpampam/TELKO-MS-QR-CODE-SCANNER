using Application.DTOs;
using Application.Features.Configurations;
using Microsoft.Extensions.Options;

namespace Application.Features.Defaults;

public class EmployeeContactDtoDefaults
{
    private readonly EmployeeContactDefaultsOptions _options;

    public EmployeeContactDtoDefaults(IOptions<EmployeeContactDefaultsOptions> options)
    {
        _options = options.Value;
    }

    public void ApplyDefaults(EmployeeContactDto dto)
    {
        if (dto == null) return;

        dto.LinkedIn = string.IsNullOrWhiteSpace(dto.LinkedIn)
            ? _options.defaultLinkedInUrl
            : dto.LinkedIn;
    }
}
using WebUI.Helpers;

namespace WebUI.DTOs.ContactRequestDtos;

public record GetAllContacts
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Phone { get; set; }

    public void MapToFilterDateConvert(FilterDateConvert dto)
    {
        dto.StartDate = HelperExtensions.ConvertToDateTimeV2(StartDate!);
        dto.EndDate = HelperExtensions.ConvertToDateTimeV2(EndDate!);
    }

    public record FilterDateConvert
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

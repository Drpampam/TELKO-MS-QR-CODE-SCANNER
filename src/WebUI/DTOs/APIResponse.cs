using System.ComponentModel.DataAnnotations;

namespace WebUI.DTOs;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public string ResponseCode { get; set; }
    public bool Success { get; set; }

}
public class PaginationResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class EmployeeContact
{
    public string? FullName { get; set; }
    public string PersonalPhone { get; set; } = string.Empty;
    public string WorkPhone { get; set; } = null!; 
    public string Email { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? LinkedIn { get; set; }

}


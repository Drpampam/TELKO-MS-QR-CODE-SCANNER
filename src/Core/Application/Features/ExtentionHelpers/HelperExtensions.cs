using System.Globalization;

namespace Application.Features.ExtentionHelpers;

public static class HelperExtensions
{
    public static string GenerateUniqueReference(this DateTime dateTime)
    {
        var timestamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
        var random = new Random();
        var randomPart = random.Next(10000000, 99999999);

        var uniqueReference = timestamp + randomPart.ToString();

        return uniqueReference;
    }

    public static DateTime? ConvertToDateTime(string inputTime)
    {
        DateTime targetTime;

        if (DateTime.TryParseExact(inputTime, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out targetTime)) return targetTime;

        if (DateTime.TryParse(inputTime, out targetTime)) return targetTime.ToLocalTime();
        return null;
    }

    public static DateTime? ConvertToDateTimeV2(string inputTime)
    {
        DateTime targetTime;

        if (DateTime.TryParseExact(inputTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out targetTime)) return targetTime;
        return null;
    }

    public static string ConvertFromDateTime(DateTime dateTime)
    {
        return dateTime.ToString("HH:mm", CultureInfo.InvariantCulture);
    }
}
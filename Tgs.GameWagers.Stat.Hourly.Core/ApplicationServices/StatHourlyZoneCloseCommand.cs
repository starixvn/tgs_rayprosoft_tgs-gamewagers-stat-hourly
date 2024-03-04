namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyZoneCloseCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    int TimeSpanMinutes,
    DateTimeOffset StartOn,
    DateTimeOffset EndOn);

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record BrandHourlyCommand(
    string BrandCode,
    DateTimeOffset StartOn,
    DateTimeOffset EndOn);

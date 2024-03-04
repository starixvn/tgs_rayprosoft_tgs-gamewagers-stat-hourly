namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record BrandHourlyAllCommand(
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);

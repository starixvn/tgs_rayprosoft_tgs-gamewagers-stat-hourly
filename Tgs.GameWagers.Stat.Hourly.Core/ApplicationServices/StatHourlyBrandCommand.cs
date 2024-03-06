namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyBrandCommand(
	string BrandCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);

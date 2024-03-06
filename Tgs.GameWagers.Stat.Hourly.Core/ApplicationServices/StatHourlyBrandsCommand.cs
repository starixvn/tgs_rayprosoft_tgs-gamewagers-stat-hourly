namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyBrandsCommand(
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyCloseCommand(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	int TimeSpanMinutes,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);

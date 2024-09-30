using MediatR;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyCloseRequest(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn) : IRequest;

using MediatR;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyBrandRequest(
	string BrandCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn) : IRequest;

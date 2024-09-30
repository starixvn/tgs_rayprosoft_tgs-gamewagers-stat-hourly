using MediatR;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

public record StatHourlyBrandsRequest(
	DateTimeOffset StartOn,
	DateTimeOffset EndOn) : IRequest;

using MediatR;

namespace Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

public record JobExecuteRequest(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode) : IRequest;

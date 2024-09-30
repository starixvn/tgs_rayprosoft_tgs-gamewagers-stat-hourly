using MediatR;
using Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class HangfireUtensil(
	IMediator mediator) : IHangfireUtensil
{
	public Task JobCheckAsync()
		=> mediator.Send(new JobCheckRequest());

	public Task JobExecuteAsync(JobExecuteRequest source)
		=> mediator.Send(source);

	public Task JobInitAsync()
		=> mediator.Send(new JobInitRequest());
}

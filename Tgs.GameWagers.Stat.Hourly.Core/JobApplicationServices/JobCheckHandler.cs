using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Text.Json;
using Tgs.GameWagers.Stat.Hourly.Core.Models;

namespace Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

internal class JobCheckHandler(
	ILogger<JobCheckHandler> logger,
	IBrandRepository repository,
	IMediator mediator) : IRequestHandler<JobCheckRequest>
{
	public async Task Handle(JobCheckRequest request, CancellationToken cancellationToken)
	{
		using var connection = JobStorage.Current.GetConnection();
		if (connection.GetRecurringJobs().Count == 0)
		{
			logger.LogError("{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Message = "Check that the schedule is not started and reset it."
				}));

			await mediator.Send(
				request: new JobInitRequest(),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return;
		}

		var brands = await repository.QueryAsync(
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<BrandInfo>();

		if (brands.Count() != connection.GetRecurringJobs().Count)
		{
			logger.LogError("{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						RecurringJobs = connection.GetRecurringJobs().Count,
						BrandCodeCount = brands.Count()
					},
					Message = "The number of RecurringJobs and BrandCode is different, reset it."
				}));

			await mediator.Send(
				request: new JobInitRequest(),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return;
		}

		logger.LogWarning("{logWarning}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Message = $"Check if the schedule is started.({connection.GetRecurringJobs().Count})"
			}));
	}
}

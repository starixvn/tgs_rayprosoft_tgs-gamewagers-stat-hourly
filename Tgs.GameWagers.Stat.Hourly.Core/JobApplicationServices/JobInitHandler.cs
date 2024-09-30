using System.Runtime.InteropServices;
using System.Text.Json;
using Hangfire;
using Hangfire.Storage;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Core.Models;

namespace Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

internal class JobInitHandler(
	ILogger<JobInitHandler> logger,
	IHangfireLock hangfireLock,
	IBrandRepository repository) : IRequestHandler<JobInitRequest>
{
	private readonly RecurringJobOptions _recurringJobOptions = new()
	{
		TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "China Standard Time" : "Asia/Shanghai")
	};

	public async Task Handle(JobInitRequest request, CancellationToken cancellationToken)
	{
		try
		{
			if (!await hangfireLock.LockingAsync(
				application: typeof(JobInitHandler).Name,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				logger.LogWarning("{logWarning}",
					JsonSerializer.Serialize(new
					{
						LogOn = DateTime.Now.ToString("u"),
						Message = "Perform initialization actions, and other services are already in progress."
					}));
				return;
			}

			using (var connection = JobStorage.Current.GetConnection())
			{
				foreach (var recurringJob in connection.GetRecurringJobs())
				{
					RecurringJob.RemoveIfExists(recurringJob.Id);
				}
			}

			var brands = await repository.QueryAsync(
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<BrandInfo>();

			foreach (var brand in brands)
			{
				var jobExecuteCommand = new JobExecuteRequest(
					ResellerId: brand.ResellerId,
					ResellerCode: brand.ResellerCode,
					LicenseeId: brand.LicenseeId,
					LicenseeCode: brand.LicenseeCode,
					BrandId: brand.BrandId,
					BrandCode: brand.BrandCode);

				RecurringJob.AddOrUpdate<IHangfireUtensil>(
					recurringJobId: $"{brand.ResellerCode}|{brand.LicenseeCode}|{brand.BrandCode}",
					methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
					cronExpression: "*/5 * * * *",
					options: _recurringJobOptions);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Message = "An exception occurred during the initialization schedule."
				}));
			return;
		}
		finally
		{
			_ = BackgroundJob.Schedule<IHangfireUtensil>(service => service.JobCheckAsync(), TimeSpan.FromMinutes(1));
		}
	}
}

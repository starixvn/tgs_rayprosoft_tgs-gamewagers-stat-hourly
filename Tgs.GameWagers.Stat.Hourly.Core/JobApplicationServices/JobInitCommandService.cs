using System.Runtime.InteropServices;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

internal class JobInitCommandService(
	ILogger<JobInitCommandService> logger,
	IHangfireLock hangfireLock,
	IBrandRepository repository) : ICommandService<JobInitCommand>
{
	private readonly RecurringJobOptions _recurringJobOptions = new()
	{
		TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "China Standard Time" : "Asia/Shanghai")
	};

	public async ValueTask ExecuteAsync(JobInitCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await hangfireLock.LockingAsync(
				application: typeof(JobInitCommandService).Name,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				logger.LogWarning("LogOn:{logOn} Message:進行初始化動作，已經有其他服務進行", DateTime.Now.ToString("o"));
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
				var jobExecuteCommand = new JobExecuteCommand(
					ResellerId: brand.ResellerId,
					ResellerCode: brand.ResellerCode,
					LicenseeId: brand.LicenseeId,
					LicenseeCode: brand.LicenseeCode,
					BrandId: brand.BrandId,
					BrandCode: brand.BrandCode,
					TimeSpanMinutes: brand.TimeSpanMinutes);

				RecurringJob.AddOrUpdate<IHangfireUtensil>(
					recurringJobId: $"{brand.ResellerCode}|{brand.LicenseeCode}|{brand.BrandCode}|{brand.TimeSpanMinutes}",
					methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
					cronExpression: "*/5 * * * *",
					options: _recurringJobOptions);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Message:初始化排程發生例外", DateTime.Now.ToString("o"));
		}
		finally
		{
			_ = BackgroundJob.Schedule<IHangfireUtensil>(service => service.JobCheckAsync(), TimeSpan.FromMinutes(1));
		}
	}
}

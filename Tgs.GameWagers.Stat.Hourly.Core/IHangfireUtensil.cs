using Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteRequest source);

	public Task JobInitAsync();
}

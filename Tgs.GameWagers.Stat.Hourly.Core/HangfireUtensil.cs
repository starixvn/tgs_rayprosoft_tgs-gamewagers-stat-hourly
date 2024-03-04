
using Tgs.ServiceUtility;
using Tgs.GameWagers.Stat.Hourly.Core;
using Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class HangfireUtensil(
    ICommandService<JobCheckCommand> jobCheckService,
    ICommandService<JobExecuteCommand> jobExecuteService,
    ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensil
{
    public Task JobCheckAsync()
        => jobCheckService.ExecuteAsync(new()).AsTask();

    public Task JobExecuteAsync(JobExecuteCommand source)
        => jobExecuteService.ExecuteAsync(source).AsTask();

    public Task JobInitAsync()
        => jobInitservice.ExecuteAsync(new()).AsTask();
}

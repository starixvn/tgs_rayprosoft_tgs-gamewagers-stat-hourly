using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IStatisticalHourlyCloseDispatcher
{
	ValueTask PushCommandAsync(StatHourlyCloseCommand command, CancellationToken cancellationToken = default);
}

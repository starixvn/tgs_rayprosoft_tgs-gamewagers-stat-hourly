using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IStatisticalHourlyZoneCloseDispatcher
{
	ValueTask PushCommandAsync(StatHourlyZoneCloseCommand command, CancellationToken cancellationToken = default);
}

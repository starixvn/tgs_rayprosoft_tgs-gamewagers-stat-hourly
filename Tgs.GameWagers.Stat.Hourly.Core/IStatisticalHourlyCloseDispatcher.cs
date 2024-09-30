using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IStatisticalHourlyCloseDispatcher
{
	ValueTask PushRequestAsync(StatHourlyCloseRequest request, CancellationToken cancellationToken = default);
}

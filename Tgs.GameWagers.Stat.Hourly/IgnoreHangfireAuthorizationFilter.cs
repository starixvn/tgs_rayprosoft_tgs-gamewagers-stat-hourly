using Hangfire.Dashboard;

namespace Tgs.GameWagers.Stat.Hourly;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}

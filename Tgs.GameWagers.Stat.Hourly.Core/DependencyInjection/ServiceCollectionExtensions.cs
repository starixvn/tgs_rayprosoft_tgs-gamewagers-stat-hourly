using System.Reflection;
using StackExchange.Redis;
using Tgs.GameWagers.Stat.Hourly.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatHourlyCore(
		this IServiceCollection services,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton(TimeProvider.System)
		.AddSingleton<IHangfireLock>(sp => ActivatorUtilities.CreateInstance<HangfireLock>(sp, redisDatabaseResolver(sp)))
		.AddScoped<IHangfireUtensil, HangfireUtensil>()
		.AddSingleton<IStatisticalHourlyCloseDispatcher, StatisticalHourlyCloseDispatcher>()
		.AddSingleton<IStatisticalHourlyZoneCloseDispatcher, StatisticalHourlyZoneCloseDispatcher>()
		.AddSingleton<IStatisticalTimeUtnsil, StatisticalTimeUtnsil>();
}

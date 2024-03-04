using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Hourly;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatHourly(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> redisOptions,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameWagersStatHourlyCore(
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database)
		.AddGameWagersStatHourlyRepositories(
			dbContextOptions: dbContextOptions);
}

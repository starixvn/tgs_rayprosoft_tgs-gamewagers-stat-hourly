using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tgs.GameWagers.Stat.Hourly.Core;
using Tgs.GameWagers.Stat.Hourly.Entities;
using Tgs.GameWagers.Stat.Hourly.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatHourlyRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddScoped<IDbConnection, NpgsqlConnection>((sp) =>
		{
			var context = sp.GetRequiredService<TgsContext>();
			return (NpgsqlConnection)context.Database.GetDbConnection();
		})
		.AddTransient<IBrandRepository, BrandRepository>()
		.AddTransient<IGameWagerStatCloseRepository, GameWagerStatCloseRepository>()
		.AddTransient<IGameWagerStatHourlyCloseRepository, GameWagerStatHourlyCloseRepository>();
}

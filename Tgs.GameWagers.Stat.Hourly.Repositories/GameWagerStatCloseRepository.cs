using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Hourly.Core;
using Tgs.GameWagers.Stat.Hourly.Entities;

namespace Tgs.GameWagers.Stat.Hourly.Repositories;
internal class GameWagerStatCloseRepository(
	TgsContext context) : IGameWagerStatCloseRepository
{
	public async ValueTask<bool> ExistAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default)
		=> await context.GameWagerStatCloses.AnyAsync(
			predicate: predicate =>
			predicate.StatOn >= startOn.UtcDateTime &&
			predicate.StatOn < endOn.UtcDateTime &&
			predicate.BrandId == brandId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}

namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IGameWagerStatHourlyCloseRepository
{
	ValueTask DeleteBatchAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);

	ValueTask InsertBatchAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}

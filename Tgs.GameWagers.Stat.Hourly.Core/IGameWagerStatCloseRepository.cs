namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IGameWagerStatCloseRepository
{
	ValueTask<bool> ExistAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}

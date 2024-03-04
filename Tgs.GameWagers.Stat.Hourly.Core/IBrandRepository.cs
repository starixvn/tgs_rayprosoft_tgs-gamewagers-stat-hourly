using Tgs.GameWagers.Stat.Hourly.Core.Models;

namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IBrandRepository
{
	IAsyncEnumerable<BrandInfo> QueryAsync(CancellationToken cancellationToken = default);

	ValueTask<BrandInfo?> GetAsync(string code, CancellationToken cancellationToken = default);
}

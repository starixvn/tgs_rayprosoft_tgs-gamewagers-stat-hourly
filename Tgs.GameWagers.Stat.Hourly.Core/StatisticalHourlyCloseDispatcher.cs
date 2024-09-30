using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class StatisticalHourlyCloseDispatcher(IServiceProvider serviceProvider) : IStatisticalHourlyCloseDispatcher, IDisposable, IAsyncDisposable
{
	private readonly ConcurrentDictionary<string, StatisticalHourlyCloseChannel> _counters = new();
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private bool _disposedValue;

	~StatisticalHourlyCloseDispatcher()
	{
		Dispose(false);
	}

	public async ValueTask PushRequestAsync(StatHourlyCloseRequest request, CancellationToken cancellationToken = default)
	{
		var key = "dispatcher";
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
		var counter = _counters.GetOrAdd(
			key,
			key =>
			{
				var scope = serviceProvider.CreateScope();
				var newCounter = ActivatorUtilities.CreateInstance<StatisticalHourlyCloseChannel>(
					scope.ServiceProvider,
					key);

				newCounter.Disposed += (_, _) =>
				{
					_ = _counters.TryRemove(newCounter.Key, out _);
					scope.Dispose();
				};

				return newCounter;
			});

		await counter.PrepareAsync(cts.Token).ConfigureAwait(false);
		await counter.PushRequestAsync(request, cts.Token).ConfigureAwait(false);
	}

	public ValueTask DisposeAsync()
	{
		Dispose(true);
		GC.SuppressFinalize(this);

		return ValueTask.CompletedTask;
	}

	public void Dispose()
	{
		// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource.Dispose();
			_disposedValue = true;
		}
	}
}

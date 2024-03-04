using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class StatisticalHourlyZoneCloseDispatcher(IServiceProvider serviceProvider) : IStatisticalHourlyZoneCloseDispatcher, IDisposable, IAsyncDisposable
{
	private readonly CancellationTokenSource _cancellationTokenSource = new();
	private readonly ConcurrentDictionary<string, StatisticalHourlyZoneCloseChannel> _counters = new();
	private bool _disposedValue;

	~StatisticalHourlyZoneCloseDispatcher()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public ValueTask DisposeAsync()
	{
		Dispose(true);
		GC.SuppressFinalize(this);

		return ValueTask.CompletedTask;
	}

	public async ValueTask PushCommandAsync(
		StatHourlyZoneCloseCommand command,
		CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
		var key = string.Join('-', new[] { command.ResellerCode, command.LicenseeCode, command.BrandCode });
		//var key = typeof(StatisticalHourlyZoneCloseDispatcher).FullName!;
		if (!_counters.TryGetValue(key, out StatisticalHourlyZoneCloseChannel? channel))
		{
			await InitAsync(
				key: key,
				cancellationToken: cts.Token)
				.ConfigureAwait(false);

			channel = _counters.GetValueOrDefault(key)!;
		}

		await channel.PushCommandAsync(command, cts.Token).ConfigureAwait(false);
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

	private ValueTask InitAsync(
		string key,
		CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
		var counter = _counters.GetOrAdd(
			key: key,
			valueFactory: key =>
			{
				var scope = serviceProvider.CreateScope();
				var newCounter = ActivatorUtilities.CreateInstance<StatisticalHourlyZoneCloseChannel>(
					scope.ServiceProvider,
					key);

				newCounter.Disposed += (_, _) =>
				{
					_ = _counters.TryRemove(newCounter.Key, out _);
					scope.Dispose();
				};

				return newCounter;
			});

		return counter.InitAsync(cts.Token);
	}
}

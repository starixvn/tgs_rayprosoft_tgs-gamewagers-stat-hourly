using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class StatisticalHourlyCloseChannel(
	ILogger<StatisticalHourlyCloseChannel> logger,
	IMediator mediator,
	string key) : IDisposable, IAsyncDisposable
{
	private static readonly ActivitySource _ActivitySource = new(name: typeof(StatisticalHourlyCloseChannel).FullName!);

	private readonly Channel<StatHourlyCloseRequest> _channel = Channel.CreateUnbounded<StatHourlyCloseRequest>(
		new UnboundedChannelOptions
		{
			SingleReader = true
		});

	private readonly SemaphoreSlim _locker = new(1, 1);
	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private Task? _backgroundTask;
	private Timer? _timer;
	private bool _disposedValue;

	public event EventHandler? Disposed;

	public string Key { get; } = key;

	~StatisticalHourlyCloseChannel()
	{
		// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
		Dispose(disposing: false);
	}

	public async ValueTask PrepareAsync(CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

		if (_backgroundTask is not null)
			return;

		await _locker.WaitAsync(cts.Token).ConfigureAwait(false);

		try
		{
			if (_backgroundTask is not null)
				return;

			_backgroundTask = Task.Run(
				() => ProcessCommandLoopAsync(_cancellationTokenSource.Token),
				_cancellationTokenSource.Token);
		}
		finally
		{
			_ = _locker.Release();
		}
	}

	public async ValueTask PushCommandAsync(StatHourlyCloseRequest request, CancellationToken cancellationToken = default)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(
			token1: cancellationToken,
			token2: _cancellationTokenSource.Token);

		await _channel.Writer.WriteAsync(
			item: request,
			cancellationToken: cts.Token).ConfigureAwait(false);
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
				_timer?.Dispose();
				_timer = null;

				Disposed?.Invoke(this, EventArgs.Empty);
				_channel.Writer.Complete();
				_cancellationTokenSource.Cancel();
			}

			_cancellationTokenSource.Dispose();
			_disposedValue = true;
		}
	}

	private async Task ProcessCommandLoopAsync(CancellationToken cancellationToken = default)
	{
		await Task.Yield();

		var reader = _channel.Reader;
		while (await reader
			.WaitToReadAsync(cancellationToken)
			.ConfigureAwait(false))
		{
			if (reader.TryRead(out var request))
			{
				using var activity = _ActivitySource.StartActivity(
					"StatisticalHourlyCloseChannel handle exchange",
					 ActivityKind.Internal,
					parentContext: default);
				try
				{
					logger.LogInformation("{logInformation}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Key,
							Data = new
							{
								request.ResellerCode,
								request.LicenseeCode,
								request.BrandCode,
								StartOn = request.StartOn.ToString("u"),
								EndOn = request.EndOn.ToString("u")
							},
							Message = "This processing."
						}));
					await mediator.Send(
						request: request,
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "{logError}",
						JsonSerializer.Serialize(new
						{
							LogOn = DateTime.Now.ToString("u"),
							Key,
							Data = new
							{
								request.ResellerCode,
								request.LicenseeCode,
								request.BrandCode,
								StartOn = request.StartOn.ToString("u"),
								EndOn = request.EndOn.ToString("u")
							},
							Message = "This exception occurs."
						}));
				}
			}
		}
	}
}
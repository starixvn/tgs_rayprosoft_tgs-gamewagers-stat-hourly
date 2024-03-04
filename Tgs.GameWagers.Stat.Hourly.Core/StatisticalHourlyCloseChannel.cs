using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

using Tgs.ServiceUtility;

using Tgs.GameWagers.Stat.Hourly.Core;

using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class StatisticalHourlyCloseChannel(
    ILogger<StatisticalHourlyCloseChannel> logger,
    ICommandService<StatHourlyCloseCommand> service,
    string key) : IDisposable
{
    private static readonly ActivitySource _ActivitySource = new(name: typeof(StatisticalHourlyCloseChannel).FullName!);

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly Channel<StatHourlyCloseCommand> _channel = Channel.CreateUnbounded<StatHourlyCloseCommand>(
        new UnboundedChannelOptions
        {
            SingleReader = true
        });

    private readonly SemaphoreSlim _locker = new(1, 1);
    private Task? _backgroundTask;
    private bool _disposedValue;
    /*
    private int _expiry = 330;
    private Timer? _timer;
    */

    public event EventHandler? Disposed;

    public string Key { get; } = key;

    ~StatisticalHourlyCloseChannel()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask InitAsync(CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

        if (_backgroundTask is not null)
            return;

        await _locker.WaitAsync(cts.Token).ConfigureAwait(false);

        try
        {
            if (_backgroundTask is not null)
            {
                return;
            }
            /*
            _timer = new Timer(_ =>
            {
                _ = Interlocked.Decrement(ref _expiry);

                if (_expiry <= 0)
                {
                    Dispose();
                }
            }, null, 0, 1000);
            */

            _backgroundTask = Task.Run(
                () => ProcessCommandLoopAsync(_cancellationTokenSource.Token),
                _cancellationTokenSource.Token);
        }
        finally
        {
            _ = _locker.Release();
        }
    }

    public async ValueTask PushCommandAsync(StatHourlyCloseCommand command, CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _cancellationTokenSource.Token);

        await _channel.Writer.WriteAsync(
            command,
            cts.Token)
            .ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                /*
                _timer?.Dispose();
                _timer = null;
                */

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
        while (await reader.WaitToReadAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false))
        {
            if (reader.TryRead(out var command))
            {
                using var activity = _ActivitySource.StartActivity(
                    "StatisticalHourlyCloseChannel handle exchange",
                     ActivityKind.Internal,
                    parentContext: default);
                try
                {
                    logger.LogInformation("LogOn:{logOn} Key:{key} Command:{command}", DateTime.Now.ToString("o"), Key, command);

                    await service.ExecuteAsync(
                        command: command,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "LogOn:{logOn} Mesage:Process command occur errors.({command})", DateTime.Now.ToString("o"), command);
                }
            }
        }
    }
}

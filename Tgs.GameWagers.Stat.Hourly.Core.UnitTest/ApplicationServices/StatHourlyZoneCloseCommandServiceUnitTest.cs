using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest.ApplicationServices;

public class StatHourlyZoneCloseCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var faceLogger = NullLogger<StatHourlyZoneCloseCommandService>.Instance;
        var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
        var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
        var fakeGameWagerStatHourlyZoneCloseRepository = Substitute.For<IGameWagerStatHourlyZoneCloseRepository>();
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new StatHourlyZoneCloseCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            TimeSpanMinutes: 480,
            StartOn: DateTimeOffset.Parse("2024-01-09T16:00:00.000+08:00"),
            EndOn: DateTimeOffset.Parse("2024-01-09T17:00:00.000+08:00"));

        var startOn = new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00));
        var endOn = new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00));
        _ = fakeStatisticalTimeUtnsil.GetTimeZoneHour(
            startOn: Arg.Is(command.StartOn),
            endOn: Arg.Is(command.EndOn),
            timeSpanMinutes: Arg.Is(480))
            .Returns((startOn, endOn));

        _ = fakeGameWagerStatCloseRepository.ExistAsync(
            brandId: Arg.Is(command.BrandId),
            startOn: Arg.Is(startOn),
            endOn: Arg.Is(endOn),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new StatHourlyZoneCloseCommandService(
            faceLogger,
            fakeStatisticalTimeUtnsil,
            fakeGameWagerStatCloseRepository,
            fakeGameWagerStatHourlyZoneCloseRepository,
            fakeMessageSender);

        await service.ExecuteAsync(command: command);

        _ = fakeGameWagerStatHourlyZoneCloseRepository
            .Received(1)
            .DeleteBatchAsync(
            brandId: Arg.Is(command.BrandId),
            startOn: Arg.Is(startOn),
            endOn: Arg.Is(endOn),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameWagerStatHourlyZoneCloseRepository
            .Received(1)
            .InsertBatchAsync(
            brandId: Arg.Is(command.BrandId),
            startOn: Arg.Is(startOn),
            endOn: Arg.Is(endOn),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            Arg.Is(GameWagersStatHourlyNatsConst.StatHourlyCloseZoneFinish),
            data: Arg.Is(new GameWagersStatHourlyFinishMessage
            {
                ResellerId = command.ResellerId,
                ResellerCode = command.ResellerCode,
                LicenseeId = command.LicenseeId,
                LicenseeCode = command.LicenseeCode,
                BrandId = command.BrandId,
                BrandCode = command.BrandCode,
                TimeSpanMinutes = command.TimeSpanMinutes,
                StartOn = startOn.ToTimestamp(),
                EndOn = endOn.ToTimestamp()
            }),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task No_Data()

    {
        var faceLogger = NullLogger<StatHourlyZoneCloseCommandService>.Instance;
        var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
        var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
        var fakeGameWagerStatHourlyZoneCloseRepository = Substitute.For<IGameWagerStatHourlyZoneCloseRepository>();
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new StatHourlyZoneCloseCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            TimeSpanMinutes: 480,
            StartOn: DateTimeOffset.Parse("2024-01-09T16:00:00.000+08:00"),
            EndOn: DateTimeOffset.Parse("2024-01-09T17:00:00.000+08:00"));

        var startOn = new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00));
        var endOn = new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00));
        _ = fakeStatisticalTimeUtnsil.GetTimeZoneHour(
            startOn: Arg.Is(command.StartOn),
            endOn: Arg.Is(command.EndOn),
            timeSpanMinutes: Arg.Is(480))
            .Returns((startOn, endOn));

        _ = fakeGameWagerStatCloseRepository.ExistAsync(
            brandId: Arg.Is(command.BrandId),
            startOn: Arg.Is(startOn),
            endOn: Arg.Is(endOn),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        var service = new StatHourlyZoneCloseCommandService(
            faceLogger,
            fakeStatisticalTimeUtnsil,
            fakeGameWagerStatCloseRepository,
            fakeGameWagerStatHourlyZoneCloseRepository,
            fakeMessageSender);

        await service.ExecuteAsync(command: command);

        _ = fakeGameWagerStatHourlyZoneCloseRepository
            .Received(0)
            .DeleteBatchAsync(
            brandId: Arg.Is(command.BrandId),
            startOn: Arg.Is(startOn),
            endOn: Arg.Is(endOn),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameWagerStatHourlyZoneCloseRepository
            .Received(0)
            .InsertBatchAsync(
            brandId: Arg.Is(command.BrandId),
            startOn: Arg.Is(startOn),
            endOn: Arg.Is(endOn),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            Arg.Is(GameWagersStatHourlyNatsConst.StatHourlyCloseZoneFinish),
            data: Arg.Is(new GameWagersStatHourlyFinishMessage
            {
                ResellerId = command.ResellerId,
                ResellerCode = command.ResellerCode,
                LicenseeId = command.LicenseeId,
                LicenseeCode = command.LicenseeCode,
                BrandId = command.BrandId,
                BrandCode = command.BrandCode,
                TimeSpanMinutes = command.TimeSpanMinutes,
                StartOn = startOn.ToTimestamp(),
                EndOn = endOn.ToTimestamp()
            }),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}

using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest.ApplicationServices;

public class StatHourlyCloseHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<StatHourlyCloseHandler>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
		var fakeGameWagerStatHourlyCloseRepository = Substitute.For<IGameWagerStatHourlyCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var request = new StatHourlyCloseRequest(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			StartOn: DateTimeOffset.Parse("2024-01-09T16:00:00.000+08:00"),
			EndOn: DateTimeOffset.Parse("2024-01-09T17:00:00.000+08:00"));

		var startOn = new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00));
		var endOn = new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00));
		_ = fakeStatisticalTimeUtnsil.GetTimeZoneHour(
			startOn: Arg.Is(request.StartOn),
			endOn: Arg.Is(request.EndOn),
			timeSpanMinutes: Arg.Is(480))
			.Returns((startOn, endOn));

		_ = fakeGameWagerStatCloseRepository.ExistAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new StatHourlyCloseHandler(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatCloseRepository,
			fakeGameWagerStatHourlyCloseRepository,
			fakeMessageSender);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameWagerStatHourlyCloseRepository
			.Received(1)
			.DeleteBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatHourlyCloseRepository
			.Received(1)
			.InsertBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			Arg.Is(GameWagersStatHourlyNatsConst.StatHourlyCloseFinish),
			data: Arg.Is(new GameWagersStatHourlyFinishMessage
			{
				ResellerId = request.ResellerId,
				ResellerCode = request.ResellerCode,
				LicenseeId = request.LicenseeId,
				LicenseeCode = request.LicenseeCode,
				BrandId = request.BrandId,
				BrandCode = request.BrandCode,
				TimeSpanMinutes = 480,
				StartOn = startOn.ToTimestamp(),
				EndOn = endOn.ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task No_Data()

	{
		var faceLogger = NullLogger<StatHourlyCloseHandler>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatCloseRepository = Substitute.For<IGameWagerStatCloseRepository>();
		var fakeGameWagerStatHourlyCloseRepository = Substitute.For<IGameWagerStatHourlyCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var request = new StatHourlyCloseRequest(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			StartOn: DateTimeOffset.Parse("2024-01-09T16:00:00.000+08:00"),
			EndOn: DateTimeOffset.Parse("2024-01-09T17:00:00.000+08:00"));

		var startOn = new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00));
		var endOn = new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00));
		_ = fakeStatisticalTimeUtnsil.GetTimeZoneHour(
			startOn: Arg.Is(request.StartOn),
			endOn: Arg.Is(request.EndOn),
			timeSpanMinutes: Arg.Is(480))
			.Returns((startOn, endOn));

		_ = fakeGameWagerStatCloseRepository.ExistAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new StatHourlyCloseHandler(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatCloseRepository,
			fakeGameWagerStatHourlyCloseRepository,
			fakeMessageSender);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameWagerStatHourlyCloseRepository
			.Received(0)
			.DeleteBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatHourlyCloseRepository
			.Received(0)
			.InsertBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			Arg.Is(GameWagersStatHourlyNatsConst.StatHourlyCloseFinish),
			data: Arg.Is(new GameWagersStatHourlyFinishMessage
			{
				ResellerId = request.ResellerId,
				ResellerCode = request.ResellerCode,
				LicenseeId = request.LicenseeId,
				LicenseeCode = request.LicenseeCode,
				BrandId = request.BrandId,
				BrandCode = request.BrandCode,
				TimeSpanMinutes = 480,
				StartOn = startOn.ToTimestamp(),
				EndOn = endOn.ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

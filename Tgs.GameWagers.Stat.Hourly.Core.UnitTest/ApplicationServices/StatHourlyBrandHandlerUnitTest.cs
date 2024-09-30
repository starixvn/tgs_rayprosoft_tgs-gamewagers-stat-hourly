using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Hourly.Core.Models;

namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest.ApplicationServices;

public class StatHourlyBrandHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<StatHourlyBrandHandler>.Instance;
		var fakeDateTimeOffsetUtensil = Substitute.For<IDateTimeOffsetUtensil>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var request = new StatHourlyBrandRequest(
			BrandCode: "GPI",
			StartOn: DateTimeOffset.Parse("2024-01-09T16:00:00.000+08:00"),
			EndOn: DateTimeOffset.Parse("2024-01-09T17:00:00.000+08:00"));

		_ = fakeDateTimeOffsetUtensil.Verify(
			Arg.Is(request.StartOn),
			Arg.Is(request.EndOn))
			.Returns(true);

		var brand = new BrandInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI");
		_ = fakeBrandRepository.GetAsync(
			code: Arg.Is(request.BrandCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(brand);

		var sut = new StatHourlyBrandHandler(
			faceLogger,
			fakeDateTimeOffsetUtensil,
			fakeBrandRepository,
			fakeMessageSender);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			Arg.Is(GameWagersStatHourlyNatsConst.StatHourlyClose),
			data: Arg.Is(new GameWagersStatHourlyMessage
			{
				ResellerId = brand.ResellerId,
				ResellerCode = brand.ResellerCode,
				LicenseeId = brand.LicenseeId,
				LicenseeCode = brand.LicenseeCode,
				BrandId = brand.BrandId,
				BrandCode = brand.BrandCode,
				StartOn = request.StartOn.ToTimestamp(),
				EndOn = request.EndOn.ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

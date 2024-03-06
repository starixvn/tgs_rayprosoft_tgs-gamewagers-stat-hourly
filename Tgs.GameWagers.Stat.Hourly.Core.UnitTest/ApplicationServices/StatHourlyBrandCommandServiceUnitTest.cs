using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Valhalla.MessageQueue;
using Tgs.GameWagers.Stat.Hourly.Core.Models;

namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest.ApplicationServices;
public class StatHourlyBrandCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<StatHourlyBrandCommandService>.Instance;
		var fakeDateTimeOffsetUtensil = Substitute.For<IDateTimeOffsetUtensil>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatHourlyBrandCommand(
			BrandCode: "GPI",
			StartOn: DateTimeOffset.Parse("2024-01-09T16:00:00.000+08:00"),
			EndOn: DateTimeOffset.Parse("2024-01-09T17:00:00.000+08:00"));

		_ = fakeDateTimeOffsetUtensil.Verify(
			Arg.Is(command.StartOn),
			Arg.Is(command.EndOn))
			.Returns(true);

		var brand = new BrandInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480);
		_ = fakeBrandRepository.GetAsync(
			code: Arg.Is(command.BrandCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(brand);

		var sut = new StatHourlyBrandCommandService(
			faceLogger,
			fakeDateTimeOffsetUtensil,
			fakeBrandRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command: command);

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
				TimeSpanMinutes = brand.TimeSpanMinutes,
				StartOn = command.StartOn.ToTimestamp(),
				EndOn = command.EndOn.ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

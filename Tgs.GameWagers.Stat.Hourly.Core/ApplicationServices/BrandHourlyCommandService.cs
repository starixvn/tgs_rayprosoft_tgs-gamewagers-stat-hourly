using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Common;

using Tgs.GameWagers.Stat.Hourly.Common.Protos;

using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

internal class BrandHourlyCommandService(
	ILogger<BrandHourlyCommandService> logger,
	IBrandRepository repository,
	IMessageSender messageSender) : ICommandService<BrandHourlyCommand>
{
	public async ValueTask ExecuteAsync(BrandHourlyCommand command, CancellationToken cancellationToken = default)
	{
		var brand = await repository.GetAsync(
			code: command.BrandCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This brand does not exist.{command.BrandCode}");

		var message = new GameWagersStatHourlyMessage
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
		};

		try
		{
			await messageSender.PublishAsync(
				subject: GameWagersStatHourlyNatsConst.StatHourlyClose,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Subject:{subject} Message:The content of this message.({message})",
				DateTime.Now.ToString("u"),
				GameWagersStatHourlyNatsConst.StatHourlyClose,
				message);
		}
	}
}

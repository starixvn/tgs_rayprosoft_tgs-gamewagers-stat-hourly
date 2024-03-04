using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

internal class StatHourlyZoneCloseCommandService(
	ILogger<StatHourlyZoneCloseCommandService> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatCloseRepository gameWagerStatClosesRepository,
	IGameWagerStatHourlyZoneCloseRepository gameWagerStatHourZoneClosesRepository,
	IMessageSender messageSender) : ICommandService<StatHourlyZoneCloseCommand>
{
	public async ValueTask ExecuteAsync(StatHourlyZoneCloseCommand command, CancellationToken cancellationToken = default)
	{
		var (StartOn, EndOn) = statisticalTimeUtnsil.GetTimeZoneHour(command.StartOn, command.EndOn, command.TimeSpanMinutes);
		try
		{
			if (await gameWagerStatClosesRepository.ExistAsync(
				brandId: command.BrandId,
				startOn: StartOn,
				endOn: EndOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				await gameWagerStatHourZoneClosesRepository.DeleteBatchAsync(
					brandId: command.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

				await gameWagerStatHourZoneClosesRepository.InsertBatchAsync(
					brandId: command.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Message: This exception occurs.({command})", DateTime.Now.ToString("o"), command);
		}
		finally
		{
			var message = new GameWagersStatHourlyFinishMessage
			{
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				TimeSpanMinutes = command.TimeSpanMinutes,
				StartOn = StartOn.ToTimestamp(),
				EndOn = EndOn.ToTimestamp()
			};

			logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:The content of this message.({message})",
				DateTime.Now.ToString("u"),
				GameWagersStatHourlyNatsConst.StatHourlyCloseZoneFinish,
				message);

			await messageSender.PublishAsync(
				subject: GameWagersStatHourlyNatsConst.StatHourlyCloseZoneFinish,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}

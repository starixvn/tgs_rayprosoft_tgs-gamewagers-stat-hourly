using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

internal class StatHourlyCloseHandler(
	ILogger<StatHourlyCloseHandler> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatCloseRepository gameWagerStatClosesRepository,
	IGameWagerStatHourlyCloseRepository gameWagerStatHourClosesRepository,
	IMessageSender messageSender) : IRequestHandler<StatHourlyCloseRequest>
{
	public async Task Handle(StatHourlyCloseRequest request, CancellationToken cancellationToken)
	{
		var timeSpanMinutes = 480;
		var (StartOn, EndOn) = statisticalTimeUtnsil.GetTimeZoneHour(request.StartOn, request.EndOn, timeSpanMinutes);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					request.ResellerCode,
					request.LicenseeCode,
					request.BrandCode,
					TimeSpanMinutes = timeSpanMinutes,
					StartOn = StartOn.ToString("u"),
					EndOn = EndOn.ToString("u")
				}
			}));
		try
		{
			if (await gameWagerStatClosesRepository.ExistAsync(
				brandId: request.BrandId,
				startOn: StartOn,
				endOn: EndOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				await gameWagerStatHourClosesRepository.DeleteBatchAsync(
					brandId: request.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

				await gameWagerStatHourClosesRepository.InsertBatchAsync(
					brandId: request.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						request.ResellerCode,
						request.LicenseeCode,
						request.BrandCode,
						TimeSpanMinutes = timeSpanMinutes,
						StartOn = StartOn.ToString("u"),
						EndOn = EndOn.ToString("u")
					},
					ex.Message
				}));
		}
		finally
		{
			await messageSender.PublishAsync(
				subject: GameWagersStatHourlyNatsConst.StatHourlyCloseFinish,
				data: new GameWagersStatHourlyFinishMessage
				{
					ResellerId = request.ResellerId,
					ResellerCode = request.ResellerCode,
					LicenseeId = request.LicenseeId,
					LicenseeCode = request.LicenseeCode,
					BrandId = request.BrandId,
					BrandCode = request.BrandCode,
					TimeSpanMinutes = timeSpanMinutes,
					StartOn = StartOn.ToTimestamp(),
					EndOn = EndOn.ToTimestamp()
				},
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}

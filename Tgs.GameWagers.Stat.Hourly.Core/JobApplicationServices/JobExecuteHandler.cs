using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.Logging;

using Tgs.GameWagers.Stat.Hourly.Common;

using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

internal class JobExecuteHandler(
	ILogger<JobExecuteHandler> logger,
	TimeProvider timeProvider,
	IStatisticalTimeUtnsil utnsil,
	IMessageSender messageSender) : IRequestHandler<JobExecuteRequest>
{

	public Task Handle(JobExecuteRequest request, CancellationToken cancellationToken)
	{
		var currentTime = timeProvider.GetUtcNow();
		var statOn = utnsil.GetStatOn(currentTime, 480);

		var message = new GameWagersStatHourlyMessage
		{
			ResellerId = request.ResellerId,
			ResellerCode = request.ResellerCode,
			LicenseeId = request.LicenseeId,
			LicenseeCode = request.LicenseeCode,
			BrandId = request.BrandId,
			BrandCode = request.BrandCode,
			StartOn = statOn.AddMinutes(-5).ToTimestamp(),
			EndOn = statOn.ToTimestamp()
		};

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					message.ResellerId,
					message.ResellerCode,
					message.LicenseeId,
					message.LicenseeCode,
					message.BrandId,
					message.BrandCode,
					StartOn = message.StartOn.ToDateTimeOffset().ToString("u"),
					EndOn = message.EndOn.ToDateTimeOffset().ToString("u")
				},
			}));

		return messageSender.PublishAsync(
			subject: GameWagersStatHourlyNatsConst.StatHourlyClose,
			data: message,
			cancellationToken: cancellationToken).AsTask();
	}
}

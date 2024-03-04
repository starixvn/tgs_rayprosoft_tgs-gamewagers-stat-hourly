using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Logging;

using Tgs.GameWagers.Stat.Hourly.Common;

using Tgs.GameWagers.Stat.Hourly.Common.Protos;

using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	TimeProvider timeProvider,
	IStatisticalTimeUtnsil utnsil,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var statOn = utnsil.GetStatOn(currentTime, 480);

		var message = new GameWagersStatHourlyMessage
		{
			ResellerId = command.ResellerId,
			ResellerCode = command.ResellerCode,
			LicenseeId = command.LicenseeId,
			LicenseeCode = command.LicenseeCode,
			BrandId = command.BrandId,
			BrandCode = command.BrandCode,
			TimeSpanMinutes = command.TimeSpanMinutes,
			StartOn = statOn.AddHours(-1).ToTimestamp(),
			EndOn = statOn.ToTimestamp()
		};

		logger.LogInformation("LogOn:{logOn} Message:The content of this message.({message})", DateTime.Now.ToString("u"), message);

		return messageSender.PublishAsync(
			subject: GameWagersStatHourlyNatsConst.StatHourlyClose,
			data: message,
			cancellationToken: cancellationToken);
	}
}

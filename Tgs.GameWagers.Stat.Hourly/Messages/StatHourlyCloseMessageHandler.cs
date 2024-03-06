using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Messages;

public class StatHourlyCloseMessageHandler(
	ILogger<StatHourlyCloseMessageHandler> logger,
	IStatisticalHourlyCloseDispatcher dispatcher) : IMessageHandler<GameWagersStatHourlyMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatHourlyMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This receiving message data info.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return dispatcher.PushCommandAsync(
			command: new StatHourlyCloseCommand(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				TimeSpanMinutes: data.TimeSpanMinutes,
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}

using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Messages;

public class StatHourlyBrandMessageHandler(
	ILogger<StatHourlyBrandMessageHandler> logger,
	ICommandService<StatHourlyBrandCommand> service) : IMessageHandler<GameWagersStatHourlyBrandMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatHourlyBrandMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This receiving message data info.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new StatHourlyBrandCommand(
				BrandCode: data.BrandCode,
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}

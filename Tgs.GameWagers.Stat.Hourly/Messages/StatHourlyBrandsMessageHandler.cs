using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Messages;

public class StatHourlyBrandsMessageHandler(
	ILogger<StatHourlyBrandsMessageHandler> logger,
	ICommandService<StatHourlyBrandsCommand> service) : IMessageHandler<GameWagersStatHourlyBrandsMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatHourlyBrandsMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This receiving message data info.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return service.ExecuteAsync(
			command: new StatHourlyBrandsCommand(
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}

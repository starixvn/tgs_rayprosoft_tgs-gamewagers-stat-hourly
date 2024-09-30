using System.Text.Json;
using MediatR;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Messages;

public class StatHourlyBrandsMessageHandler(
	ILogger<StatHourlyBrandsMessageHandler> logger,
	IMediator mediator) : IMessageHandler<GameWagersStatHourlyBrandsMessage>
{
	public async ValueTask HandleAsync(string subject, GameWagersStatHourlyBrandsMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					StartOn = data.StartOn.ToDateTimeOffset().ToString("u"),
					EndOn = data.EndOn.ToDateTimeOffset().ToString("u")
				},
				Message = "This receive message handler."
			}));

		await mediator.Send(
			request: new StatHourlyBrandsRequest(
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}

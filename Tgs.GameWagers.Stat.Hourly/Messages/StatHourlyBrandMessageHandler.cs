using System.Text.Json;
using MediatR;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Messages;

public class StatHourlyBrandMessageHandler(
	ILogger<StatHourlyBrandMessageHandler> logger,
	IMediator mediator) : IMessageHandler<GameWagersStatHourlyBrandMessage>
{
	public async ValueTask HandleAsync(string subject, GameWagersStatHourlyBrandMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.BrandCode,
					StartOn = data.StartOn.ToDateTimeOffset().ToString("u"),
					EndOn = data.EndOn.ToDateTimeOffset().ToString("u")
				},
				Message = "This receive message handler."
			}));

		await mediator.Send(
			request: new StatHourlyBrandRequest(
				BrandCode: data.BrandCode,
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}

using System.Text.Json;
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
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.ResellerCode,
					data.LicenseeCode,
					data.BrandCode,
					StartOn = data.StartOn.ToDateTimeOffset().ToString("u"),
					EndOn = data.EndOn.ToDateTimeOffset().ToString("u")
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushRequestAsync(
			request: new StatHourlyCloseRequest(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}

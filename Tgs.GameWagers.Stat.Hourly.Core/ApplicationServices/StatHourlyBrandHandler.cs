using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Common;

using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

internal class StatHourlyBrandHandler(
	ILogger<StatHourlyBrandHandler> logger,
	IDateTimeOffsetUtensil dateTimeOffsetUtensil,
	IBrandRepository repository,
	IMessageSender messageSender) : IRequestHandler<StatHourlyBrandRequest>
{
	public async Task Handle(StatHourlyBrandRequest request, CancellationToken cancellationToken)
	{
		if (!dateTimeOffsetUtensil.Verify(request.StartOn, request.EndOn))
		{
			logger.LogWarning("{logWarning}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						request.BrandCode,
						StartOn = request.StartOn.ToString("u"),
						EndOn = request.EndOn.ToString("u")
					},
					Message = "This start time and end time verification failed."
				}));

			return;
		}

		var brand = await repository.GetAsync(
			code: request.BrandCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(request.BrandCode);

		var message = new GameWagersStatHourlyMessage
		{
			ResellerId = brand.ResellerId,
			ResellerCode = brand.ResellerCode,
			LicenseeId = brand.LicenseeId,
			LicenseeCode = brand.LicenseeCode,
			BrandId = brand.BrandId,
			BrandCode = brand.BrandCode,
			StartOn = request.StartOn.ToTimestamp(),
			EndOn = request.EndOn.ToTimestamp()
		};

		try
		{
			await messageSender.PublishAsync(
				subject: GameWagersStatHourlyNatsConst.StatHourlyClose,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						message.ResellerCode,
						message.LicenseeCode,
						message.BrandCode,
						StartOn = request.StartOn.ToString("u"),
						EndOn = request.EndOn.ToString("u")
					},
					ex.Message
				}));
		}
	}
}

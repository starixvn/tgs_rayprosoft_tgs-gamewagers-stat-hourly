using System;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.Core.Models;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;

internal class StatHourlyBrandsCommandService(
	ILogger<StatHourlyBrandsCommandService> logger,
	IDateTimeOffsetUtensil dateTimeOffsetUtensil,
	IBrandRepository repository,
	IMessageSender messageSender) : ICommandService<StatHourlyBrandsCommand>
{
	public async ValueTask ExecuteAsync(StatHourlyBrandsCommand command, CancellationToken cancellationToken = default)
	{
		if (!dateTimeOffsetUtensil.Verify(command.StartOn, command.EndOn))
		{
			logger.LogWarning("LogOn:{logOn}  Message:This start time({startOn}) and end time({endOn}) verification failed..",
				DateTime.Now.ToString("o"),
				command.StartOn,
				command.EndOn);
			return;
		}

		var brands = await repository.QueryAsync(
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<BrandInfo>();

		foreach (var brand in brands)
		{
			var message = new GameWagersStatHourlyMessage
			{
				ResellerId = brand.ResellerId,
				ResellerCode = brand.ResellerCode,
				LicenseeId = brand.LicenseeId,
				LicenseeCode = brand.LicenseeCode,
				BrandId = brand.BrandId,
				BrandCode = brand.BrandCode,
				TimeSpanMinutes = brand.TimeSpanMinutes,
				StartOn = command.StartOn.ToTimestamp(),
				EndOn = command.EndOn.ToTimestamp()
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
				logger.LogError(ex, "LogOn:{logOn} Subject:{subject} Message:The content of this message.({message})",
					DateTime.Now.ToString("u"),
					GameWagersStatHourlyNatsConst.StatHourlyClose,
					message);
			}
		}
	}
}

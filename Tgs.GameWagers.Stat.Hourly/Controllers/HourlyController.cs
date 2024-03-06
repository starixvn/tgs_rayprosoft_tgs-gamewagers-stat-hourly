using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Hourly.Common;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Tgs.GameWagers.Stat.Hourly.ViewModels;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Hourly.Controllers;

[Route("[controller]")]
[ApiController]
public class HourlyController : ControllerBase
{
	/// <summary>
	///
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="brandCode">The brand code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post /GPI/
	///         {
	///             "startOn": "2024-03-05T00:00:00.000+08:00",
	///             "endOn": "2024-03-06T00:00:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPost("{brandCode}", Name = "StatHourly_Brand")]
	public ValueTask StatHourlyBrandAsync(
		[FromServices] IMessageSender messageSender,
		string brandCode,
		HourlyViewModel source)
		=> messageSender.PublishAsync(
			subject: GameWagersStatHourlyNatsConst.StatHourlyBrand,
			data: new GameWagersStatHourlyBrandMessage
			{
				BrandCode = brandCode,
				StartOn = source.StartOn.ToTimestamp(),
				EndOn = source.EndOn.ToTimestamp()
			},
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	///
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "startOn": "2024-03-05T00:00:00.000+08:00",
	///             "endOn": "2024-03-06T00:00:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPost("all", Name = "StatHourly_Brands")]
	public ValueTask StatHourlyBrandsAsync(
		[FromServices] IMessageSender messageSender,
		HourlyViewModel source)
		=> messageSender.PublishAsync(
			subject: GameWagersStatHourlyNatsConst.StatHourlyBrands,
			data: new GameWagersStatHourlyBrandsMessage
			{
				StartOn = source.StartOn.ToTimestamp(),
				EndOn = source.EndOn.ToTimestamp()
			},
			cancellationToken: HttpContext.RequestAborted);
}

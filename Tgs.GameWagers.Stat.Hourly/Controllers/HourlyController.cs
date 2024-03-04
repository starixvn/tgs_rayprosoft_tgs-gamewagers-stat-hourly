using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Hourly.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Hourly.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameWagers.Stat.Hourly.Controllers;
[Route("[controller]")]
[ApiController]
public class HourlyController : ControllerBase
{
	/// <summary>
	/// Brands the hour asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="brandCode">The brand code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post /GPI/
	///         {
	///             "startOn": "2023-09-01T00:00:00.000+08:00",
	///             "endOn": "2024-01-01T00:00:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPost("{brandCode}", Name = "BrandHour")]
	public ValueTask BrandHourAsync(
		[FromServices] ICommandService<BrandHourlyCommand> service,
		string brandCode,
		HourlyViewModel source)
	{
		var command = new BrandHourlyCommand
		(
			BrandCode: brandCode,
			StartOn: source.StartOn,
			EndOn: source.EndOn
		);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: HttpContext.RequestAborted);
	}

	/// <summary>
	/// Brands the hour all asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "startOn": "2023-09-01T00:00:00.000+08:00",
	///             "endOn": "2024-01-01T00:00:00.000+08:00"
	///         }
	/// </remarks>
	[HttpPost("all", Name = "BrandHourAll")]
	public ValueTask BrandHourAllAsync(
		[FromServices] ICommandService<BrandHourlyAllCommand> service,
		HourlyViewModel source)
	{
		var command = new BrandHourlyAllCommand
		(
			StartOn: source.StartOn,
			EndOn: source.EndOn
		);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: HttpContext.RequestAborted);
	}
}

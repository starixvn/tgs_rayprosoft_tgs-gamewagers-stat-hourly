using Microsoft.AspNetCore.Mvc;
using Tgs.GameWagers.Stat.Hourly.Core;
using Tgs.GameWagers.Stat.Hourly.Core.Models;

namespace Tgs.GameWagers.Stat.Hourly.Controllers;

[Route("[controller]")]
[ApiController]
public class BrandController : ControllerBase
{
	[HttpGet()]
	public IAsyncEnumerable<BrandInfo> QueryAsync(
		[FromServices] IBrandRepository repository)
		=> repository.QueryAsync(HttpContext.RequestAborted);

	[HttpGet("{code}")]
	public ValueTask<BrandInfo?> GetAsync(
		[FromServices] IBrandRepository repository,
		string code)
		=> repository.GetAsync(code, HttpContext.RequestAborted);
}

using System.ComponentModel.DataAnnotations;
using Tgs.GameWagers.Stat.Hourly.Attributes;

namespace Tgs.GameWagers.Stat.Hourly.ViewModels;

public class HourlyViewModel
{
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartOn { get; init; }

	[Required]
	[DataType(DataType.DateTime)]
	[DateTimeRangeDay("StartOn", 31)]
	public DateTimeOffset EndOn { get; init; }
}

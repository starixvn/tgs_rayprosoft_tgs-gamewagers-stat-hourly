#nullable disable

using System.ComponentModel.DataAnnotations;

namespace Tgs.GameWagers.Stat.Hourly.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DateTimeRangeDayAttribute(string parameter, int max) : ValidationAttribute
{
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		var endTime = (DateTimeOffset)value;

		var startTime = (DateTimeOffset)validationContext.ObjectType.GetProperty(parameter).GetValue(validationContext.ObjectInstance, null);

		TimeSpan diffTimeSpan = endTime.Subtract(startTime);

		return startTime < endTime && diffTimeSpan.TotalDays <= max ?
			ValidationResult.Success :
			new ValidationResult($"The start time cannot be greater than the end time, and the time range is limited({max}day).");
	}
}

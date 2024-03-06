using Microsoft.Extensions.Logging;

namespace Tgs.GameWagers.Stat.Hourly.Core;
internal class DateTimeOffsetUtensil(
	ILogger<DateTimeOffsetUtensil> logger,
	TimeProvider timeProvider) : IDateTimeOffsetUtensil
{
	public bool Verify(DateTimeOffset startOn, DateTimeOffset endOn)
	{
		if (startOn.CompareTo(endOn) != -1)
		{
			logger.LogWarning("LogOn:{logOn}  Message:This start time({startOn}) is greater than the end time({endOn}).",
				DateTime.Now.ToString("o"),
				startOn,
				endOn);
			return false;
		}

		var diffSubtract = startOn.Subtract(endOn);

		if (diffSubtract.TotalDays > 31)
		{
			logger.LogWarning("LogOn:{logOn}  Message:The operation interval between this start time({startOn}) and end time({endOn}) exceeds 31 days.",
				DateTime.Now.ToString("o"),
				startOn,
				endOn);
			return false;
		}

		var currentTime = timeProvider.GetUtcNow();

		diffSubtract = currentTime.Subtract(startOn);

		if (diffSubtract.TotalDays > 60)
		{
			logger.LogWarning("LogOn:{logOn}  Message:This start time({startOn}) is more than 60 days from the current time({currentTime}).",
				DateTime.Now.ToString("o"),
				startOn,
				currentTime);
			return false;
		}

		return true;
	}
}

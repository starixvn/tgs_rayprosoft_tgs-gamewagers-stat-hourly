namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class StatisticalTimeUtnsil() : IStatisticalTimeUtnsil
{
	public DateTimeOffset GetStatOn(DateTimeOffset source, int timeSpanMinutes = 480)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		var sourceTimeSpan = source.ToOffset(timeSpan);
		var mins = sourceTimeSpan.Minute % 5;
		var temp = sourceTimeSpan.AddMinutes(-mins);
		return new DateTimeOffset(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, 00, timeSpan);
	}

	public (DateTimeOffset StartOn, DateTimeOffset EndOn) GetTimeZoneHour(DateTimeOffset startOn, DateTimeOffset endOn, int timeSpanMinutes = 480)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		var startOnTimeSpan = startOn.ToOffset(timeSpan);
		var endOnTimeSpan = endOn.ToOffset(timeSpan).AddHours(+1);
		return new(
			new DateTimeOffset(startOnTimeSpan.Year, startOnTimeSpan.Month, startOnTimeSpan.Day, startOnTimeSpan.Hour, 00, 00, timeSpan),
			new DateTimeOffset(endOnTimeSpan.Year, endOnTimeSpan.Month, endOnTimeSpan.Day, endOnTimeSpan.Hour, 00, 00, timeSpan));
	}
}

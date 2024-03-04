namespace Tgs.GameWagers.Stat.Hourly.Core;

internal class StatisticalTimeUtnsil() : IStatisticalTimeUtnsil
{
	public (DateTimeOffset StartOn, DateTimeOffset EndOn) GetTimeZoneHour(DateTimeOffset startOn, DateTimeOffset endOn, int timeSpanMinutes)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		var startOnTimeSpan = startOn.ToOffset(timeSpan);
		var endOnTimeSpan = endOn.ToOffset(timeSpan).AddHours(+1);
		return new(
			new DateTimeOffset(startOnTimeSpan.Year, startOnTimeSpan.Month, startOnTimeSpan.Day, startOnTimeSpan.Hour, 00, 00, timeSpan),
			new DateTimeOffset(endOnTimeSpan.Year, endOnTimeSpan.Month, endOnTimeSpan.Day, endOnTimeSpan.Hour, 00, 00, timeSpan));
	}

	public (DateTimeOffset StartOn, DateTimeOffset EndOn) GetTimeZoneHour(DateTimeOffset source, int timeSpanMinutes = 480)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		var sourceTimeSpan = source.ToOffset(timeSpan);
		var mode = sourceTimeSpan.Minute % 5;
		var startOn = sourceTimeSpan.AddMinutes(mode * -1);
		var endOn = sourceTimeSpan.AddMinutes(5 - mode).AddHours(1);
		return new(
			new DateTimeOffset(startOn.Year, startOn.Month, startOn.Day, startOn.Hour, 00, 00, timeSpan),
			new DateTimeOffset(endOn.Year, endOn.Month, endOn.Day, endOn.Hour, 00, 00, timeSpan));
	}
}

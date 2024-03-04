namespace Tgs.GameWagers.Stat.Hourly.Core;

public interface IStatisticalTimeUtnsil
{
	DateTimeOffset GetStatOn(DateTimeOffset source, int timeSpanMinutes = 480);

	(DateTimeOffset StartOn, DateTimeOffset EndOn) GetTimeZoneHour(DateTimeOffset startOn, DateTimeOffset endOn, int timeSpanMinutes = 480);
}

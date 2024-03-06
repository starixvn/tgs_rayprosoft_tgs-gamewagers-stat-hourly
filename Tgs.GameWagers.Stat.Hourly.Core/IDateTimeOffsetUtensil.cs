namespace Tgs.GameWagers.Stat.Hourly.Core;
public interface IDateTimeOffsetUtensil
{
	public bool Verify(DateTimeOffset startOn, DateTimeOffset endOn);
}

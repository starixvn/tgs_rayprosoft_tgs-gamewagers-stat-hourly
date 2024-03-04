using NSubstitute;

namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest;

public class StatisticalTimeUtnsilUnitTest
{
	[Fact]
	public void Get_TimeZone_Hour_23_11_21_09_54_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T09:54:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T09:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T10:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_23_11_21_09_55_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T09:55:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T09:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T11:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_23_11_21_09_56_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T09:56:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T09:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T11:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_23_11_21_10_01_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:01:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T10:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T11:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_23_11_21_10_31_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:31:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T10:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T11:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_23_11_21_10_59_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:59:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T10:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T12:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Minus_240()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), -240);

		Assert.Equal(DateTimeOffset.Parse("2023-11-20T22:00:00-04:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T00:00:00-04:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Plus_330()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), 330);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T08:00:00+05:30").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T09:00:00+05:30").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Plus_540()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), 540);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T11:00:00+09:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T13:00:00+09:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Hour_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Plus_480()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneHour(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), 540);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T10:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T12:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}
}
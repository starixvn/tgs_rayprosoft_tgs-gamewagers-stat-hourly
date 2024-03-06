namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest;

public class StatisticalTimeUtnsilUnitTest
{
	[Fact]
	public void Get_StatOn_24_03_04_15_56_33_08()
	{
		var sut = new StatisticalTimeUtnsil();

		var source = new DateTimeOffset(2024, 03, 04, 15, 56, 33, TimeSpan.FromHours(+8));
		var statOn = sut.GetStatOn(source, 480);

		Assert.Equal(new DateTimeOffset(2024, 03, 04, 15, 55, 00, TimeSpan.FromHours(+8)), statOn);
	}

	[Fact]
	public void Get_StatOn_24_03_04_15_56_33_00()
	{
		var sut = new StatisticalTimeUtnsil();

		var source = new DateTimeOffset(2024, 03, 04, 15, 56, 33, TimeSpan.FromHours(0));
		var statOn = sut.GetStatOn(source, 480);

		Assert.Equal(new DateTimeOffset(2024, 03, 04, 23, 55, 00, TimeSpan.FromHours(+8)), statOn);
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

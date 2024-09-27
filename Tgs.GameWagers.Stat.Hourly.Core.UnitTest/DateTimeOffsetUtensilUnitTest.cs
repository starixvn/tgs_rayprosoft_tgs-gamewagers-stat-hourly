using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Tgs.GameWagers.Stat.Hourly.Core.UnitTest;

public class DateTimeOffsetUtensilUnitTest
{
	[Fact]
	public void Normal_Process()
	{
		var faceLogger = NullLogger<DateTimeOffsetUtensil>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var startOn = new DateTimeOffset(2024, 02, 01, 00, 00, 00, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 03, 01, 00, 00, 00, TimeSpan.FromHours(+8));

		var currentTime = new DateTimeOffset(2024, 03, 06, 05, 59, 19, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new DateTimeOffsetUtensil(
			faceLogger,
			fakeTimeProvider);

		var actual = sut.Verify(startOn, endOn);
		Assert.True(actual);
	}

	[Fact]
	public void This_start_time_is_greater_than_the_end_time()
	{
		var faceLogger = NullLogger<DateTimeOffsetUtensil>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var startOn = new DateTimeOffset(2024, 03, 01, 00, 00, 01, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 03, 01, 00, 00, 00, TimeSpan.FromHours(+8));

		var currentTime = new DateTimeOffset(2024, 03, 06, 05, 59, 19, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new DateTimeOffsetUtensil(
			faceLogger,
			fakeTimeProvider);

		var actual = sut.Verify(startOn, endOn);

		Assert.False(actual);
	}

	[Fact]
	public void The_operation_interval_between_this_start_time_and_end_time_exceeds_31_days()
	{
		var faceLogger = NullLogger<DateTimeOffsetUtensil>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var startOn = new DateTimeOffset(2024, 02, 01, 00, 00, 00, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 03, 03, 00, 00, 01, TimeSpan.FromHours(+8));

		var currentTime = new DateTimeOffset(2024, 03, 06, 05, 59, 19, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new DateTimeOffsetUtensil(
			faceLogger,
			fakeTimeProvider);

		var actual = sut.Verify(startOn, endOn);
		Assert.False(actual);
	}

	/*
	[Fact]
	public void This_start_time_is_more_than_180_days_from_the_current_time()
	{
		var faceLogger = NullLogger<DateTimeOffsetUtensil>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var startOn = new DateTimeOffset(2023, 08, 01, 00, 00, 00, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2023, 09, 01, 00, 00, 00, TimeSpan.FromHours(+8));

		var currentTime = new DateTimeOffset(2024, 03, 06, 05, 59, 19, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new DateTimeOffsetUtensil(
			faceLogger,
			fakeTimeProvider);

		var actual = sut.Verify(startOn, endOn);
		Assert.False(actual);
	}
	*/

	[Fact]
	public void This_start_time_is_more_than_204_02_01()
	{
		var faceLogger = NullLogger<DateTimeOffsetUtensil>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var startOn = new DateTimeOffset(2024, 01, 31, 00, 00, 00, TimeSpan.FromHours(+8));
		var endOn = new DateTimeOffset(2024, 02, 01, 00, 00, 00, TimeSpan.FromHours(+8));

		var currentTime = new DateTimeOffset(2024, 03, 06, 05, 59, 19, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
			.Returns(currentTime);

		var sut = new DateTimeOffsetUtensil(
			faceLogger,
			fakeTimeProvider);

		var actual = sut.Verify(startOn, endOn);
		Assert.False(actual);
	}
}

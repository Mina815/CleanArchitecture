using CleanArchitecture.Application.Common.Scheduling;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Scheduling;

public class BookingScheduleRulesTests
{
    [Test]
    public void IntervalsOverlap_ShouldDetectOverlap()
    {
        var open = new TimeOnly(10, 0);
        var close = new TimeOnly(12, 0);

        Assert.That(BookingScheduleRules.IntervalsOverlap(open, close, new TimeOnly(11, 0), new TimeOnly(13, 0)), Is.True);
        Assert.That(BookingScheduleRules.IntervalsOverlap(open, close, new TimeOnly(12, 0), new TimeOnly(13, 0)), Is.False);
    }
}

using CleanArchitecture.Domain.Enums;
using NUnit.Framework;

namespace CleanArchitecture.Domain.UnitTests.Enums;

public class BookingStatusTests
{
    [Test]
    public void Cancelled_ShouldHaveStableValueForPersistence()
    {
        Assert.That((int)BookingStatus.Cancelled, Is.EqualTo(2));
    }
}

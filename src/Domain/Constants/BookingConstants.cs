using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Constants;

public static class BookingConstants
{
    public const int MinimumCancellationHours = 24;
    public const int DefaultSlotDurationMinutes = 30;
    public const int MaxBookingsPerDay = 20;
    public const int BookingReminderHours = 24;
}

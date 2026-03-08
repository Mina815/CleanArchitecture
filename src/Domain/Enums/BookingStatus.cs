using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Enums;

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4,
    NoShow = 5
}

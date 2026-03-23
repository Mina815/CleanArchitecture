using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Common.Scheduling;

public static class BookingScheduleRules
{
    public static TimeOnly AddMinutes(TimeOnly start, int minutes) =>
        start.Add(TimeSpan.FromMinutes(minutes));

    public static bool IntervalsOverlap(TimeOnly aStart, TimeOnly aEnd, TimeOnly bStart, TimeOnly bEnd) =>
        aStart < bEnd && bStart < aEnd;

    public static bool IsBookingDuringTimeOff(DateOnly date, TimeOnly rangeStart, TimeOnly rangeEnd, TimeOff off)
    {
        if (date < off.FromDate || date > off.ToDate)
            return false;

        if (off.FromTime is null || off.ToTime is null)
            return true;

        return IntervalsOverlap(rangeStart, rangeEnd, off.FromTime.Value, off.ToTime.Value);
    }

    public static void ValidateNewBooking(
        Service service,
        Branch branch,
        WorkingHour? daySchedule,
        IEnumerable<Booking> existingBookings,
        IEnumerable<TimeOff> timeOffs,
        DateOnly bookingDate,
        TimeOnly startTime,
        int? staffId)
    {
        if (!service.IsActive)
            throw new SlotNotAvailableException("Service is not available.");

        if (!branch.IsActive || branch.CenterId != service.CenterId)
            throw new SlotNotAvailableException("Branch is not valid for this service.");

        if (daySchedule is null || daySchedule.IsClosed)
            throw new SlotNotAvailableException("Branch is closed on this day.");

        var endTime = AddMinutes(startTime, service.DurationMinutes);

        if (startTime < daySchedule.OpenTime || endTime > daySchedule.CloseTime)
            throw new SlotNotAvailableException("Requested time is outside working hours.");

        foreach (var b in existingBookings.Where(x => x.Status != BookingStatus.Cancelled))
        {
            if (IntervalsOverlap(startTime, endTime, b.StartTime, b.EndTime))
                throw new SlotNotAvailableException("This time slot is already booked.");
        }

        foreach (var off in timeOffs)
        {
            if (off.StaffId is not null && staffId is not null && off.StaffId != staffId)
                continue;

            if (off.StaffId is not null && staffId is null)
                continue;

            if (IsBookingDuringTimeOff(bookingDate, startTime, endTime, off))
                throw new SlotNotAvailableException("Branch or staff is unavailable at this time.");
        }
    }

    public static IReadOnlyList<TimeOnly> GetAvailableStarts(
        DateOnly bookingDate,
        int serviceDurationMinutes,
        WorkingHour? daySchedule,
        IReadOnlyList<Booking> existingBookings,
        IReadOnlyList<TimeOff> timeOffs,
        int? staffId,
        int slotStepMinutes)
    {
        if (daySchedule is null || daySchedule.IsClosed)
            return [];

        var result = new List<TimeOnly>();
        var open = daySchedule.OpenTime;
        var close = daySchedule.CloseTime;

        for (var t = open; AddMinutes(t, serviceDurationMinutes) <= close; t = AddMinutes(t, slotStepMinutes))
        {
            var end = AddMinutes(t, serviceDurationMinutes);
            var blockedByBooking = existingBookings
                .Where(b => b.Status != BookingStatus.Cancelled)
                .Any(b => IntervalsOverlap(t, end, b.StartTime, b.EndTime));

            if (blockedByBooking)
                continue;

            var blockedByTimeOff = timeOffs.Any(off =>
            {
                if (off.StaffId is not null && staffId is not null && off.StaffId != staffId)
                    return false;
                if (off.StaffId is not null && staffId is null)
                    return false;
                return IsBookingDuringTimeOff(bookingDate, t, end, off);
            });

            if (blockedByTimeOff)
                continue;

            result.Add(t);
        }

        return result;
    }
}

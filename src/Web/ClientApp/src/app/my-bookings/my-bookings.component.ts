import { Component, OnInit, inject } from '@angular/core';
import { BookingsClient, BookingDto } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html'
})
export class MyBookingsComponent implements OnInit {
  private bookingsClient = inject(BookingsClient);

  bookings: BookingDto[] = [];
  loading = false;
  showCancelDialog = false;
  cancelBookingId: number | null = null;
  cancelReason = '';
  filter: 'upcoming' | 'past' | 'all' = 'upcoming';

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.loading = true;
    this.bookingsClient.getMyBookings(undefined).subscribe({
      next: result => {
        this.bookings = result ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  filteredBookings(): BookingDto[] {
    const now = new Date();
    return this.bookings.filter(b => {
      const bDate = b.bookingDate ? new Date(b.bookingDate) : null;
      if (!bDate) return this.filter === 'all';
      const isUpcoming = bDate >= now || b.status === 'Pending' || b.status === 'Confirmed';
      if (this.filter === 'upcoming') return isUpcoming;
      if (this.filter === 'past') return !isUpcoming;
      return true;
    });
  }

  cancel(id: number): void {
    this.cancelBookingId = id;
    this.cancelReason = '';
    this.showCancelDialog = true;
  }

  confirmCancel(): void {
    if (!this.cancelBookingId) return;
    this.bookingsClient.cancelBooking(this.cancelBookingId, this.cancelReason || undefined).subscribe({
      next: () => {
        this.showCancelDialog = false;
        this.cancelBookingId = null;
        this.cancelReason = '';
        this.loadBookings();
      }
    });
  }

  getStatusLabel(status: string | undefined): string {
    switch (status) {
      case 'Pending': return 'قيد الانتظار';
      case 'Confirmed': return 'مؤكد';
      case 'InProgress': return 'قيد التنفيذ';
      case 'Cancelled': return 'ملغي';
      case 'Completed': return 'مكتمل';
      default: return status ?? '';
    }
  }

  getStatusClass(status: string | undefined): string {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'Confirmed': return 'status-confirmed';
      case 'InProgress': return 'status-in-progress';
      case 'Cancelled': return 'status-cancelled';
      case 'Completed': return 'status-completed';
      default: return '';
    }
  }

  formatDate(dateStr: Date | undefined): string {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('ar-SA', { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' });
  }

  isUpcoming(booking: BookingDto): boolean {
    if (!booking.bookingDate) return false;
    return new Date(booking.bookingDate) >= new Date();
  }

  canCancel(booking: BookingDto): boolean {
    return (booking.status === 'Pending' || booking.status === 'Confirmed') && this.isUpcoming(booking);
  }
}

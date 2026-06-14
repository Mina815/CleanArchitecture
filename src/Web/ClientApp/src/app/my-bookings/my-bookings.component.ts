import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { BookingDto, JamalekApiService } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html'
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingDto[] = [];
  loading = true;
  error = '';
  private loadingTimeout?: ReturnType<typeof setTimeout>;

  constructor(private api: JamalekApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    this.api.getMyBookings().subscribe({
      next: data => { clearTimeout(this.loadingTimeout); this.bookings = data; this.loading = false; this.cdr.detectChanges(); },
      error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Failed to load bookings.'; this.cdr.detectChanges(); }
    });
  }

  cancel(id: number): void {
    if (!confirm('Cancel this booking? Cancellations must be at least 24 hours before the appointment.')) return;
    this.api.cancelBooking(id, 'Cancelled by customer').subscribe({
      next: () => this.load(),
      error: () => alert('Could not cancel. You may be within the 24-hour window.')
    });
  }

  canCancel(b: BookingDto): boolean {
    return b.status === 'Pending' || b.status === 'Confirmed';
  }

  statusClass(status: string): string {
    return 'badge--' + status.toLowerCase();
  }
}

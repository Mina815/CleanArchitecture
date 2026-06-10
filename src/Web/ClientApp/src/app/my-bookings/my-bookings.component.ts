import { Component, OnInit } from '@angular/core';
import { BookingDto, JamalekApiService } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html'
})
export class MyBookingsComponent implements OnInit {
  bookings: BookingDto[] = [];
  loading = true;

  constructor(private api: JamalekApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getMyBookings().subscribe({
      next: data => { this.bookings = data; this.loading = false; },
      error: () => this.loading = false
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

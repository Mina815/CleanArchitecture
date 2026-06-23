import { Injectable, computed, inject, signal } from '@angular/core';
import { BookingDto, BookingDetailDto, BookingsClient } from '../web-api-client';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export type AsyncState = 'idle' | 'loading' | 'success' | 'error';

@Injectable({ providedIn: 'root' })
export class BookingStore {
  private readonly bookingsClient = inject(BookingsClient);

  private readonly listState = signal<AsyncState>('idle');
  private readonly listData = signal<BookingDto[]>([]);
  private readonly detailData = signal<BookingDetailDto | null>(null);
  private readonly detailState = signal<AsyncState>('idle');

  readonly bookings = this.listData.asReadonly();
  readonly listStatus = this.listState.asReadonly();
  readonly isLoading = computed(() => this.listState() === 'loading');
  readonly isLoaded = computed(() => this.listState() === 'success');

  readonly bookingDetail = this.detailData.asReadonly();
  readonly detailStatus = this.detailState.asReadonly();

  loadBranchToday(branchId: number): Observable<BookingDto[]> {
    this.listState.set('loading');
    const today = new Date();
    return this.bookingsClient.getBranchBookings(branchId, today, today, undefined, false).pipe(
      tap({
        next: items => { this.listData.set(items ?? []); this.listState.set('success'); },
        error: () => this.listState.set('error')
      })
    );
  }

  loadMyBookings(upcoming?: boolean): Observable<BookingDto[]> {
    this.listState.set('loading');
    return this.bookingsClient.getMyBookings(upcoming).pipe(
      tap({
        next: items => { this.listData.set(items ?? []); this.listState.set('success'); },
        error: () => this.listState.set('error')
      })
    );
  }

  loadDetail(id: number): Observable<BookingDetailDto> {
    this.detailState.set('loading');
    return this.bookingsClient.getBookingById(id).pipe(
      tap({
        next: item => { this.detailData.set(item); this.detailState.set('success'); },
        error: () => this.detailState.set('error')
      })
    );
  }

  pushBooking(booking: BookingDto): void {
    this.listData.update(items => [booking, ...items]);
  }

  updateBookingStatus(bookingId: number, status: string): void {
    this.listData.update(items => items.map(b => b.id === bookingId ? { ...b, status } as BookingDto : b));
    this.detailData.update(d => d && d.id === bookingId ? { ...d, status } as BookingDetailDto : d);
  }

  removeBooking(bookingId: number): void {
    this.listData.update(items => items.filter(b => b.id !== bookingId));
  }

  reset(): void {
    this.listState.set('idle');
    this.listData.set([]);
    this.detailState.set('idle');
    this.detailData.set(null);
  }
}

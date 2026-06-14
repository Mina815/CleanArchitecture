import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { BookingDto, BranchSummaryDto, JamalekApiService } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-provider-dashboard',
  templateUrl: './provider-dashboard.component.html'
})
export class ProviderDashboardComponent implements OnInit, OnDestroy {
  branches: BranchSummaryDto[] = [];
  branchId = 0;
  bookings: BookingDto[] = [];
  loading = false;
  error = '';

  private destroy$ = new Subject<void>();
  private refreshTimer?: ReturnType<typeof setInterval>;

  constructor(private api: JamalekApiService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadCenter();
    this.refreshTimer = setInterval(() => this.loadBookings(), 30000);
  }

  ngOnDestroy(): void {
    if (this.refreshTimer) clearInterval(this.refreshTimer);
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCenter(): void {
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();

    this.api.getCenters().pipe(
      switchMap(centers => {
        if (!centers.length) {
          throw new Error('NO_CENTER');
        }
        return this.api.getCenter(centers[0].id);
      }),
      takeUntil(this.destroy$)
    ).subscribe({
      next: detail => {
        this.branches = detail.branches;
        this.branchId = detail.branches[0]?.id ?? 0;
        this.loading = false;
        this.cdr.detectChanges();
        if (this.branchId) {
          this.loadBookings();
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message === 'NO_CENTER'
          ? 'No center found. Create a center first.'
          : 'Failed to load center details.';
        this.cdr.detectChanges();
      }
    });
  }

  loadBookings(): void {
    if (!this.branchId) {
      this.loadCenter();
      return;
    }
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();

    this.api.getBranchTodayBookings(this.branchId).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: data => {
        this.bookings = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.error = 'Failed to load bookings.';
        this.cdr.detectChanges();
      }
    });
  }

  load(): void {
    this.loadBookings();
  }

  get pendingCount(): number {
    return this.bookings.filter(b => b.status === 'Pending').length;
  }

  get confirmedCount(): number {
    return this.bookings.filter(b => b.status === 'Confirmed').length;
  }

  confirm(id: number): void {
    this.api.confirmBooking(id).subscribe(() => this.loadBookings());
  }

  complete(id: number): void {
    this.api.completeBooking(id).subscribe(() => this.loadBookings());
  }

  statusClass(status: string): string {
    return 'badge--' + status.toLowerCase();
  }
}

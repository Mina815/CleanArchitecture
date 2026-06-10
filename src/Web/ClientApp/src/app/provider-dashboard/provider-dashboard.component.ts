import { Component, OnInit, OnDestroy } from '@angular/core';
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
  private refreshTimer?: ReturnType<typeof setInterval>;

  constructor(private api: JamalekApiService) {}

  ngOnInit(): void {
    this.api.getCenters().subscribe(centers => {
      if (centers.length > 0) {
        this.api.getCenter(centers[0].id).subscribe(detail => {
          this.branches = detail.branches;
          this.branchId = detail.branches[0]?.id ?? 0;
          this.load();
        });
      }
    });
    this.refreshTimer = setInterval(() => this.load(), 30000);
  }

  ngOnDestroy(): void {
    if (this.refreshTimer) clearInterval(this.refreshTimer);
  }

  load(): void {
    if (!this.branchId) return;
    this.loading = true;
    this.api.getBranchTodayBookings(this.branchId).subscribe({
      next: data => { this.bookings = data; this.loading = false; },
      error: () => this.loading = false
    });
  }

  get pendingCount(): number {
    return this.bookings.filter(b => b.status === 'Pending').length;
  }

  get confirmedCount(): number {
    return this.bookings.filter(b => b.status === 'Confirmed').length;
  }

  confirm(id: number): void {
    this.api.confirmBooking(id).subscribe(() => this.load());
  }

  complete(id: number): void {
    this.api.completeBooking(id).subscribe(() => this.load());
  }

  statusClass(status: string): string {
    return 'badge--' + status.toLowerCase();
  }
}

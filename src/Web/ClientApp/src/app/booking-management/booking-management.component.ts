import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BranchesClient, BookingsClient, BookingDetailDto } from '../web-api-client';
import { BookingStore } from '../stores/booking.store';
import { BookingHubService } from '../services/booking-hub.service';
import { CenterStore } from '../stores/center.store';

@Component({
  standalone: false,
  selector: 'app-booking-management',
  templateUrl: './booking-management.component.html'
})
export class BookingManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private readonly branchesClient = inject(BranchesClient);
  private readonly bookingsClient = inject(BookingsClient);
  private readonly hubService = inject(BookingHubService);
  readonly store = inject(BookingStore);
  readonly centerStore = inject(CenterStore);

  branches: any[] = [];
  selectedBranchId: number | null = null;
  selectedDetail: BookingDetailDto | null = null;
  confirming = false;
  completing = false;

  ngOnInit(): void {
    const center = this.centerStore.center();
    if (center?.id) {
      this.branchesClient.getBranches(center.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: items => this.branches = items ?? []
      });
    }
  }

  ngOnDestroy(): void {
    this.hubService.disconnect();
    this.destroy$.next();
    this.destroy$.complete();
  }

  selectBranch(branchId: number): void {
    this.selectedBranchId = branchId;
    this.selectedDetail = null;
    this.store.loadBranchToday(branchId).pipe(takeUntil(this.destroy$)).subscribe();
    this.hubService.connect('', branchId);
  }

  viewDetail(id: number): void {
    this.store.loadDetail(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: d => this.selectedDetail = d
    });
  }

  confirmBooking(id: number): void {
    if (this.confirming) return;
    this.confirming = true;
    this.bookingsClient.confirmBooking(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.store.updateBookingStatus(id, 'Confirmed'); this.confirming = false; if (this.selectedDetail?.id === id) this.viewDetail(id); },
      error: () => this.confirming = false
    });
  }

  completeBooking(id: number): void {
    if (this.completing) return;
    this.completing = true;
    this.bookingsClient.completeBooking(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => { this.store.updateBookingStatus(id, 'Completed'); this.completing = false; if (this.selectedDetail?.id === id) this.viewDetail(id); },
      error: () => this.completing = false
    });
  }

  statusLabel(s: string): string {
    const map: Record<string, string> = { Pending: 'قيد الانتظار', Confirmed: 'مؤكد', Cancelled: 'ملغي', Completed: 'مكتمل', NoShow: 'لم يحضر' };
    return map[s] || s;
  }
}

import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BranchesClient, BookingsClient, CentersClient, CenterDetailDto, BookingDto, BookingDetailDto } from '../web-api-client';
import { BookingHubService } from '../services/booking-hub.service';

@Component({
  standalone: false,
  selector: 'app-booking-management',
  templateUrl: './booking-management.component.html'
})
export class BookingManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  center: CenterDetailDto | null = null;
  branches: any[] = [];
  selectedBranchId: number | null = null;
  bookings: BookingDto[] = [];
  loading = false;
  selectedDetail: BookingDetailDto | null = null;
  confirming = false;
  completing = false;

  constructor(
    private branchesClient: BranchesClient,
    private bookingsClient: BookingsClient,
    private centersClient: CentersClient,
    private hubService: BookingHubService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCenter();
  }

  ngOnDestroy(): void {
    this.hubService.disconnect();
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCenter(): void {
    this.centersClient.getMyCenterEndpoint().pipe(takeUntil(this.destroy$)).subscribe({
      next: c => {
        this.center = c;
        this.cdr.detectChanges();
        if (c?.id) this.loadBranches(c.id);
      },
      error: () => this.cdr.detectChanges()
    });
  }

  loadBranches(centerId: number): void {
    this.branchesClient.getBranches(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: items => {
        this.branches = items ?? [];
        this.cdr.detectChanges();
      },
      error: () => this.cdr.detectChanges()
    });
  }

  selectBranch(branchId: number): void {
    this.selectedBranchId = branchId;
    this.selectedDetail = null;
    this.cdr.detectChanges();
    this.loadBranchBookings(branchId);
    this.hubService.connect('', branchId);
  }

  loadBranchBookings(branchId: number): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.bookingsClient.getBranchBookingsToday(branchId).pipe(takeUntil(this.destroy$)).subscribe({
      next: items => {
        this.bookings = items ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  viewDetail(id: number): void {
    this.bookingsClient.getBookingById(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: d => {
        this.selectedDetail = d;
        this.cdr.detectChanges();
      },
      error: () => this.cdr.detectChanges()
    });
  }

  confirmBooking(id: number): void {
    if (this.confirming) return;
    this.confirming = true;
    this.cdr.detectChanges();
    this.bookingsClient.confirmBooking(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.confirming = false;
        this.cdr.detectChanges();
        if (this.selectedBranchId) this.loadBranchBookings(this.selectedBranchId);
        if (this.selectedDetail?.id === id) this.viewDetail(id);
      },
      error: () => { this.confirming = false; this.cdr.detectChanges(); }
    });
  }

  completeBooking(id: number): void {
    if (this.completing) return;
    this.completing = true;
    this.cdr.detectChanges();
    this.bookingsClient.completeBooking(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.completing = false;
        this.cdr.detectChanges();
        if (this.selectedBranchId) this.loadBranchBookings(this.selectedBranchId);
        if (this.selectedDetail?.id === id) this.viewDetail(id);
      },
      error: () => { this.completing = false; this.cdr.detectChanges(); }
    });
  }

  statusLabel(s: string): string {
    const map: Record<string, string> = { Pending: 'قيد الانتظار', Confirmed: 'مؤكد', Cancelled: 'ملغي', Completed: 'مكتمل', NoShow: 'لم يحضر' };
    return map[s] || s;
  }
}

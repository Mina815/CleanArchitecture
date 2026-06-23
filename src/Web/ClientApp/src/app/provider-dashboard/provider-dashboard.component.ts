import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BookingsClient, BranchesClient, CentersClient, CenterDetailDto, BookingDto } from '../web-api-client';

interface DashboardStats {
  total: number;
  pending: number;
  confirmed: number;
  inProgress: number;
  cancelled: number;
  completed: number;
}

@Component({
  standalone: false,
  selector: 'app-provider-dashboard',
  templateUrl: './provider-dashboard.component.html'
})
export class ProviderDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  center: CenterDetailDto | null = null;
  profileComplete = false;
  branches: any[] = [];
  selectedBranchId: number | null = null;
  todayBookings: BookingDto[] = [];
  loading = false;
  stats: DashboardStats = { total: 0, pending: 0, confirmed: 0, inProgress: 0, cancelled: 0, completed: 0 };

  constructor(
    private bookingsClient: BookingsClient,
    private branchesClient: BranchesClient,
    private centersClient: CentersClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCenter();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCenter(): void {
    this.centersClient.getMyCenterEndpoint().pipe(takeUntil(this.destroy$)).subscribe({
      next: c => {
        this.center = c;
        this.profileComplete = !!(c.name && c.nameAr && c.description && c.descriptionAr && c.logoUrl);
        this.cdr.detectChanges();
        if (c?.id) this.loadBranches(c.id);
      },
      error: () => this.cdr.detectChanges()
    });
  }

  loadBranches(centerId: number): void {
    this.branchesClient.getBranches(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.branches = result ?? [];
        this.cdr.detectChanges();
        if (this.branches.length > 0) {
          this.selectBranch(this.branches[0].id!);
        }
      },
      error: () => this.cdr.detectChanges()
    });
  }

  selectBranch(branchId: number): void {
    this.selectedBranchId = branchId;
    this.cdr.detectChanges();
    this.loadBranchBookings(branchId);
  }

  loadBranchBookings(branchId: number): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.bookingsClient.getBranchBookingsToday(branchId).pipe(takeUntil(this.destroy$)).subscribe({
      next: items => {
        this.todayBookings = items ?? [];
        this.computeStats();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  computeStats(): void {
    const total = this.todayBookings.length;
    const pending = this.todayBookings.filter(b => b.status === 'Pending').length;
    const confirmed = this.todayBookings.filter(b => b.status === 'Confirmed').length;
    const inProgress = 0;
    const cancelled = this.todayBookings.filter(b => b.status === 'Cancelled').length;
    const completed = this.todayBookings.filter(b => b.status === 'Completed').length;
    this.stats = { total, pending, confirmed, inProgress, cancelled, completed };
  }

  refresh(): void {
    if (this.selectedBranchId) {
      this.loadBranchBookings(this.selectedBranchId);
    }
  }

  getStatusLabel(status?: string): string {
    const map: Record<string, string> = { Pending: 'قيد الانتظار', Confirmed: 'مؤكد', Cancelled: 'ملغي', Completed: 'مكتمل', NoShow: 'لم يحضر' };
    return status ? (map[status] || status) : '';
  }

  getStatusClass(status?: string): string {
    const map: Record<string, string> = { Pending: 'badge--pending', Confirmed: 'badge--active', Cancelled: 'badge--inactive', Completed: 'badge--completed' };
    return status ? (map[status] || '') : '';
  }
}

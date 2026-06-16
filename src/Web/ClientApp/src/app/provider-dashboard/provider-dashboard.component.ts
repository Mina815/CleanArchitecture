import { Component, OnInit, inject } from '@angular/core';
import { BookingsClient, CentersClient, BranchesClient, CenterDto, BranchDto, BookingDto } from '../web-api-client';

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
export class ProviderDashboardComponent implements OnInit {
  private bookingsClient = inject(BookingsClient);
  private centersClient = inject(CentersClient);
  private branchesClient = inject(BranchesClient);

  centers: CenterDto[] = [];
  selectedCenterId: number | null = null;
  branches: BranchDto[] = [];
  selectedBranchId: number | null = null;
  todayBookings: BookingDto[] = [];
  loading = false;
  stats: DashboardStats = { total: 0, pending: 0, confirmed: 0, inProgress: 0, cancelled: 0, completed: 0 };

  ngOnInit(): void {
    this.loadCenters();
  }

  loadCenters(): void {
    this.centersClient.getCenters(undefined, undefined, 1, 100).subscribe({
      next: result => {
        this.centers = result.items ?? [];
      }
    });
  }

  selectCenter(centerId: number): void {
    this.selectedCenterId = centerId;
    this.selectedBranchId = null;
    this.todayBookings = [];
    this.stats = { total: 0, pending: 0, confirmed: 0, inProgress: 0, cancelled: 0, completed: 0 };
    this.branchesClient.getBranches(centerId).subscribe({
      next: result => {
        this.branches = result ?? [];
      }
    });
  }

  selectBranch(branchId: number): void {
    this.selectedBranchId = branchId;
    this.loadBranchBookings(branchId);
  }

  loadBranchBookings(branchId: number): void {
    this.loading = true;
    this.bookingsClient.getBranchBookingsToday(branchId).subscribe({
      next: result => {
        this.todayBookings = result ?? [];
        this.calculateStats();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  calculateStats(): void {
    this.stats = {
      total: this.todayBookings.length,
      pending: this.todayBookings.filter(b => b.status === 'Pending').length,
      confirmed: this.todayBookings.filter(b => b.status === 'Confirmed').length,
      inProgress: this.todayBookings.filter(b => b.status === 'InProgress').length,
      cancelled: this.todayBookings.filter(b => b.status === 'Cancelled').length,
      completed: this.todayBookings.filter(b => b.status === 'Completed').length
    };
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

  refresh(): void {
    if (this.selectedBranchId) {
      this.loadBranchBookings(this.selectedBranchId);
    }
  }
}

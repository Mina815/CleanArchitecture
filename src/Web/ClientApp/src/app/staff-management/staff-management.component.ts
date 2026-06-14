import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { BranchSummaryDto, CreateStaffRequest, JamalekApiService, StaffDto, UpdateStaffRequest } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-staff-management',
  templateUrl: './staff-management.component.html'
})
export class StaffManagementComponent implements OnInit {
  centerId = 0;
  branches: BranchSummaryDto[] = [];
  selectedBranchId = 0;
  staff: StaffDto[] = [];
  loading = false;
  saving = false;
  error = '';
  success = '';
  private loadingTimeout?: ReturnType<typeof setTimeout>;
  showForm = false;
  editingStaffId: number | null = null;
  form: CreateStaffRequest = this.emptyForm();

  constructor(private api: JamalekApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    this.api.getCenters().subscribe({
      next: centers => {
        clearTimeout(this.loadingTimeout);
        if (!centers.length) {
          this.loading = false;
          this.error = 'No center found. Create a center first.';
          this.cdr.detectChanges();
          return;
        }
        this.centerId = centers[0].id;
        this.api.getCenter(this.centerId).subscribe({
          next: detail => {
            clearTimeout(this.loadingTimeout);
            this.branches = detail.branches;
            this.selectedBranchId = this.branches[0]?.id ?? 0;
            this.loading = false;
            this.cdr.detectChanges();
            this.loadStaff();
          },
          error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Failed to load center details.'; this.cdr.detectChanges(); }
        });
      },
      error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Failed to load centers.'; this.cdr.detectChanges(); }
    });
  }

  loadStaff(): void {
    if (!this.selectedBranchId) return;
    this.loading = true;
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    this.api.getBranchStaff(this.selectedBranchId).subscribe({
      next: data => { clearTimeout(this.loadingTimeout); this.staff = data; this.loading = false; this.cdr.detectChanges(); },
      error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Failed to load staff.'; this.cdr.detectChanges(); }
    });
  }

  onBranchChange(): void {
    this.staff = [];
    this.cancelForm();
    this.loadStaff();
  }

  startCreate(): void {
    this.editingStaffId = null;
    this.form = this.emptyForm();
    this.showForm = true;
    this.clearMessages();
  }

  startEdit(s: StaffDto): void {
    this.editingStaffId = s.id;
    this.form = {
      branchId: this.selectedBranchId,
      name: s.name,
      phone: s.phone ?? '',
      imageUrl: s.imageUrl ?? '',
      specialization: s.specialization ?? ''
    };
    this.showForm = true;
    this.clearMessages();
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingStaffId = null;
    this.form = this.emptyForm();
    this.clearMessages();
  }

  save(): void {
    if (!this.selectedBranchId) return;
    this.saving = true;
    this.clearMessages();

    if (this.editingStaffId) {
      const payload: UpdateStaffRequest = {
        id: this.editingStaffId,
        name: this.form.name,
        phone: this.form.phone || undefined,
        specialization: this.form.specialization || undefined,
        isActive: true
      };
      this.api.updateStaff(this.editingStaffId, payload).subscribe({
        next: () => {
          this.saving = false;
          this.success = 'Staff updated successfully.';
          this.cancelForm();
          this.loadStaff();
        },
        error: err => {
          this.saving = false;
          this.error = this.extractError(err, 'Failed to update staff.');
        }
      });
    } else {
      const payload: CreateStaffRequest = {
        branchId: this.selectedBranchId,
        name: this.form.name,
        phone: this.form.phone || undefined,
        imageUrl: this.form.imageUrl || undefined,
        specialization: this.form.specialization || undefined
      };
      this.api.createStaff(payload).subscribe({
        next: () => {
          this.saving = false;
          this.success = 'Staff created successfully.';
          this.cancelForm();
          this.loadStaff();
        },
        error: err => {
          this.saving = false;
          this.error = this.extractError(err, 'Failed to create staff.');
        }
      });
    }
  }

  private emptyForm(): CreateStaffRequest {
    return { branchId: 0, name: '', phone: '', imageUrl: '', specialization: '' };
  }

  private clearMessages(): void {
    this.error = '';
    this.success = '';
  }

  private extractError(err: unknown, fallback: string): string {
    const body = (err as { error?: { title?: string; detail?: string; errors?: Record<string, string[]> } })?.error;
    if (body?.errors) return Object.values(body.errors).flat().join(' ');
    return body?.detail || body?.title || fallback;
  }
}

import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CategoryDto, CreateServiceRequest, JamalekApiService, ServiceDto, UpdateServiceRequest } from '../services/jamalek-api.service';

@Component({
  standalone: false,
  selector: 'app-services-management',
  templateUrl: './services-management.component.html'
})
export class ServicesManagementComponent implements OnInit {
  centerId = 0;
  services: ServiceDto[] = [];
  categories: CategoryDto[] = [];
  loading = false;
  saving = false;
  error = '';
  success = '';
  private loadingTimeout?: ReturnType<typeof setTimeout>;
  showForm = false;
  editingServiceId: number | null = null;
  form: CreateServiceRequest = this.emptyForm();

  constructor(private api: JamalekApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    this.api.getServiceCategories().subscribe({
      next: c => { this.categories = c; this.cdr.detectChanges(); },
      error: () => {}
    });
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
        this.loadServices();
      },
      error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Failed to load centers.'; this.cdr.detectChanges(); }
    });
  }

  loadServices(): void {
    if (!this.centerId) return;
    this.loading = true;
    this.cdr.detectChanges();
    this.loadingTimeout = setTimeout(() => { this.loading = false; this.error = 'Server not responding. Try again.'; this.cdr.detectChanges(); }, 15000);
    this.api.getCenterServices(this.centerId).subscribe({
      next: data => { clearTimeout(this.loadingTimeout); this.services = data; this.loading = false; this.cdr.detectChanges(); },
      error: () => { clearTimeout(this.loadingTimeout); this.loading = false; this.error = 'Failed to load services.'; this.cdr.detectChanges(); }
    });
  }

  startCreate(): void {
    this.editingServiceId = null;
    this.form = this.emptyForm();
    this.showForm = true;
    this.clearMessages();
  }

  startEdit(s: ServiceDto): void {
    this.editingServiceId = s.id;
    this.form = {
      centerId: this.centerId,
      categoryId: s.categoryId,
      name: s.name,
      nameAr: s.nameAr,
      description: s.description ?? '',
      descriptionAr: s.descriptionAr ?? '',
      price: s.price,
      durationMinutes: s.durationMinutes,
      imageUrl: s.imageUrl ?? '',
      displayOrder: s.displayOrder
    };
    this.showForm = true;
    this.clearMessages();
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingServiceId = null;
    this.form = this.emptyForm();
    this.clearMessages();
  }

  save(): void {
    if (!this.centerId) return;
    this.saving = true;
    this.clearMessages();

    if (this.editingServiceId) {
      const payload: UpdateServiceRequest = {
        id: this.editingServiceId,
        name: this.form.name,
        nameAr: this.form.nameAr,
        price: this.form.price,
        durationMinutes: this.form.durationMinutes,
        isActive: true
      };
      this.api.updateService(this.editingServiceId, payload).subscribe({
        next: () => {
          this.saving = false;
          this.success = 'Service updated successfully.';
          this.cancelForm();
          this.loadServices();
        },
        error: err => {
          this.saving = false;
          this.error = this.extractError(err, 'Failed to update service.');
        }
      });
    } else {
      const payload: CreateServiceRequest = {
        centerId: this.centerId,
        categoryId: this.form.categoryId,
        name: this.form.name,
        nameAr: this.form.nameAr,
        description: this.form.description || undefined,
        descriptionAr: this.form.descriptionAr || undefined,
        price: this.form.price,
        durationMinutes: this.form.durationMinutes,
        imageUrl: this.form.imageUrl || undefined,
        displayOrder: this.form.displayOrder
      };
      this.api.createService(payload).subscribe({
        next: () => {
          this.saving = false;
          this.success = 'Service created successfully.';
          this.cancelForm();
          this.loadServices();
        },
        error: err => {
          this.saving = false;
          this.error = this.extractError(err, 'Failed to create service.');
        }
      });
    }
  }

  deleteService(id: number): void {
    if (!confirm('Delete this service?')) return;
    this.api.deleteService(id).subscribe({
      next: () => {
        this.success = 'Service deleted successfully.';
        this.loadServices();
      },
      error: err => {
        this.error = this.extractError(err, 'Failed to delete service.');
      }
    });
  }

  getCategoryName(categoryId: number): string {
    return this.categories.find(c => c.id === categoryId)?.name ?? '-';
  }

  private emptyForm(): CreateServiceRequest {
    return {
      centerId: 0,
      categoryId: 0,
      name: '',
      nameAr: '',
      price: 0,
      durationMinutes: 30,
      displayOrder: 0
    };
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

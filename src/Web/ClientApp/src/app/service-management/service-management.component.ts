import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ServicesClient, CentersClient, ServiceDto, CreateServiceCommand, UpdateServiceCommand } from '../web-api-client';
import { ServiceStore } from '../stores/service.store';
import { CenterStore } from '../stores/center.store';

@Component({
  standalone: false,
  selector: 'app-service-management',
  templateUrl: './service-management.component.html'
})
export class ServiceManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private readonly servicesClient = inject(ServicesClient);
  readonly store = inject(ServiceStore);
  readonly centerStore = inject(CenterStore);

  showForm = false;
  editItem: ServiceDto | null = null;
  formName = '';
  formNameAr = '';
  formDescription = '';
  formDescriptionAr = '';
  formPrice = 0;
  formDurationMinutes = 60;
  formDisplayOrder = 0;
  saving = false;

  ngOnInit(): void {
    const center = this.centerStore.center();
    if (center?.id) this.store.load(center.id);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  openCreate(): void {
    this.editItem = null;
    this.formName = ''; this.formNameAr = ''; this.formDescription = ''; this.formDescriptionAr = '';
    this.formPrice = 0; this.formDurationMinutes = 60; this.formDisplayOrder = 0;
    this.showForm = true;
  }

  openEdit(svc: ServiceDto): void {
    this.editItem = svc;
    this.formName = svc.name ?? ''; this.formNameAr = svc.nameAr ?? '';
    this.formDescription = svc.description ?? ''; this.formDescriptionAr = svc.descriptionAr ?? '';
    this.formPrice = svc.price ?? 0; this.formDurationMinutes = svc.durationMinutes ?? 60;
    this.formDisplayOrder = svc.displayOrder ?? 0;
    this.showForm = true;
  }

  cancelForm(): void { this.showForm = false; this.editItem = null; }

  save(): void {
    const center = this.centerStore.center();
    if (!center?.id || !this.formName || !this.formNameAr) return;
    this.saving = true;

    if (this.editItem) {
      this.servicesClient.updateService(this.editItem.id!, new UpdateServiceCommand({
        id: this.editItem.id, name: this.formName || undefined, nameAr: this.formNameAr || undefined,
        description: this.formDescription || undefined, descriptionAr: this.formDescriptionAr || undefined,
        price: this.formPrice, durationMinutes: this.formDurationMinutes, displayOrder: this.formDisplayOrder
      })).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => { this.saving = false; this.showForm = false; this.store.load(center.id!); },
        error: () => this.saving = false
      });
    } else {
      this.servicesClient.createService(new CreateServiceCommand({
        centerId: center.id, name: this.formName, nameAr: this.formNameAr,
        description: this.formDescription || undefined, descriptionAr: this.formDescriptionAr || undefined,
        price: this.formPrice, durationMinutes: this.formDurationMinutes, displayOrder: this.formDisplayOrder
      })).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => { this.saving = false; this.showForm = false; this.store.load(center.id!); },
        error: () => this.saving = false
      });
    }
  }

  toggleActive(svc: ServiceDto): void {
    const center = this.centerStore.center();
    if (!center?.id) return;
    this.servicesClient.updateService(svc.id!, new UpdateServiceCommand({
      id: svc.id, isActive: !svc.isActive
    })).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => this.store.load(center.id!)
    });
  }
}

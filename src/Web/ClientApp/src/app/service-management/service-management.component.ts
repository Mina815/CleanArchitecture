import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ServicesClient, CentersClient, ServiceDto, CenterDetailDto, CreateServiceCommand, UpdateServiceCommand } from '../web-api-client';

@Component({
  standalone: false,
  selector: 'app-service-management',
  templateUrl: './service-management.component.html'
})
export class ServiceManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  center: CenterDetailDto | null = null;
  services: ServiceDto[] = [];
  loading = false;

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

  constructor(
    private servicesClient: ServicesClient,
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
        this.cdr.detectChanges();
        if (c?.id) this.loadServices(c.id);
      },
      error: () => {
        this.cdr.detectChanges();
      }
    });
  }

  loadServices(centerId: number): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.servicesClient.getServices(centerId).pipe(takeUntil(this.destroy$)).subscribe({
      next: result => {
        this.services = result ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
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
    if (!this.center?.id || !this.formName || !this.formNameAr) return;
    this.saving = true;
    this.cdr.detectChanges();

    if (this.editItem) {
      this.servicesClient.updateService(this.editItem.id!, new UpdateServiceCommand({
        id: this.editItem.id, name: this.formName || undefined, nameAr: this.formNameAr || undefined,
        description: this.formDescription || undefined, descriptionAr: this.formDescriptionAr || undefined,
        price: this.formPrice, durationMinutes: this.formDurationMinutes, displayOrder: this.formDisplayOrder
      })).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => { this.saving = false; this.showForm = false; this.cdr.detectChanges(); this.loadServices(this.center!.id!); },
        error: () => { this.saving = false; this.cdr.detectChanges(); }
      });
    } else {
      this.servicesClient.createService(new CreateServiceCommand({
        centerId: this.center.id, name: this.formName, nameAr: this.formNameAr,
        description: this.formDescription || undefined, descriptionAr: this.formDescriptionAr || undefined,
        price: this.formPrice, durationMinutes: this.formDurationMinutes, displayOrder: this.formDisplayOrder
      })).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => { this.saving = false; this.showForm = false; this.cdr.detectChanges(); this.loadServices(this.center!.id!); },
        error: () => { this.saving = false; this.cdr.detectChanges(); }
      });
    }
  }

  toggleActive(svc: ServiceDto): void {
    if (!this.center?.id) return;
    this.servicesClient.updateService(svc.id!, new UpdateServiceCommand({
      id: svc.id, isActive: !svc.isActive
    })).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => this.loadServices(this.center!.id!),
      error: () => this.cdr.detectChanges()
    });
  }
}

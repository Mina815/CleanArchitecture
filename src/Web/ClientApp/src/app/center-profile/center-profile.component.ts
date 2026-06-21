import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CentersClient, UploadsClient, UpdateCenterCommand, FileParameter } from '../web-api-client';
import { CenterStore } from '../stores/center.store';

@Component({
  standalone: false,
  selector: 'app-center-profile',
  templateUrl: './center-profile.component.html'
})
export class CenterProfileComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private readonly centersClient = inject(CentersClient);
  private readonly uploadsClient = inject(UploadsClient);
  readonly store = inject(CenterStore);

  name = '';
  nameAr = '';
  description = '';
  descriptionAr = '';
  logoUrl = '';
  saving = false;
  uploadSaving = false;

  ngOnInit(): void {
    const c = this.store.center();
    if (c) {
      this.name = c.name ?? '';
      this.nameAr = c.nameAr ?? '';
      this.description = c.description ?? '';
      this.descriptionAr = c.descriptionAr ?? '';
      this.logoUrl = c.logoUrl ?? '';
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const file = input.files[0];
    this.uploadSaving = true;
    this.uploadsClient.uploadFile({ data: file, fileName: file.name } as FileParameter).pipe(takeUntil(this.destroy$)).subscribe({
      next: url => { this.logoUrl = url; this.uploadSaving = false; },
      error: () => this.uploadSaving = false
    });
  }

  save(): void {
    const center = this.store.center();
    if (!center?.id) return;
    this.saving = true;
    const cmd = new UpdateCenterCommand({
      id: center.id,
      name: this.name || undefined,
      nameAr: this.nameAr || undefined,
      description: this.description || undefined,
      descriptionAr: this.descriptionAr || undefined,
      logoUrl: this.logoUrl || undefined
    });
    this.centersClient.updateCenter(center.id, cmd).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.store.setCenter({ ...center as any, name: this.name, nameAr: this.nameAr, description: this.description, descriptionAr: this.descriptionAr, logoUrl: this.logoUrl, isProfileComplete: !!(this.name && this.nameAr && this.description && this.descriptionAr && this.logoUrl) } as any);
        this.saving = false;
      },
      error: () => this.saving = false
    });
  }
}

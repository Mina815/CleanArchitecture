import { Injectable, computed, inject, signal } from '@angular/core';
import { CentersClient, CenterDetailDto } from '../web-api-client';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export type AsyncState = 'idle' | 'loading' | 'success' | 'error';

@Injectable({ providedIn: 'root' })
export class CenterStore {
  private readonly centersClient = inject(CentersClient);

  private readonly state = signal<AsyncState>('idle');
  private readonly data = signal<CenterDetailDto | null>(null);
  private readonly errorMsg = signal<string | null>(null);

  readonly center = this.data.asReadonly();
  readonly status = this.state.asReadonly();
  readonly error = this.errorMsg.asReadonly();
  readonly isLoading = computed(() => this.state() === 'loading');
  readonly isLoaded = computed(() => this.state() === 'success');
  readonly hasError = computed(() => this.state() === 'error');

  readonly isProfileComplete = computed(() => {
    const c = this.data();
    return c ? !!(c.name && c.nameAr && c.description && c.descriptionAr && c.logoUrl) : false;
  });

  load(): Observable<CenterDetailDto> {
    this.state.set('loading');
    this.errorMsg.set(null);
    return this.centersClient.getMyCenterEndpoint().pipe(
      tap({
        next: c => {
          this.data.set(c);
          this.state.set('success');
        },
        error: (err) => {
          this.state.set('error');
          this.errorMsg.set(err.message ?? 'Failed to load center');
        }
      })
    );
  }

  setCenter(center: CenterDetailDto): void {
    this.data.set(center);
    this.state.set('success');
  }

  updateFromDto(center: CenterDetailDto): void {
    this.data.set(center);
  }

  reset(): void {
    this.state.set('idle');
    this.data.set(null);
    this.errorMsg.set(null);
  }
}

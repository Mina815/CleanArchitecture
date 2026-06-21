import { Injectable, computed, inject, signal } from '@angular/core';
import { ServiceDto, ServicesClient } from '../web-api-client';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export type AsyncState = 'idle' | 'loading' | 'success' | 'error';

@Injectable({ providedIn: 'root' })
export class ServiceStore {
  private readonly servicesClient = inject(ServicesClient);

  private readonly state = signal<AsyncState>('idle');
  private readonly data = signal<ServiceDto[]>([]);

  readonly services = this.data.asReadonly();
  readonly status = this.state.asReadonly();
  readonly isLoading = computed(() => this.state() === 'loading');
  readonly isLoaded = computed(() => this.state() === 'success');

  load(centerId: number): Observable<ServiceDto[]> {
    this.state.set('loading');
    return this.servicesClient.getServices(centerId).pipe(
      tap({
        next: items => { this.data.set(items ?? []); this.state.set('success'); },
        error: () => this.state.set('error')
      })
    );
  }

  setServices(items: ServiceDto[]): void {
    this.data.set(items);
    this.state.set('success');
  }

  reset(): void {
    this.state.set('idle');
    this.data.set([]);
  }
}

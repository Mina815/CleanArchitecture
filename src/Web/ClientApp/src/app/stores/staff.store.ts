import { Injectable, computed, inject, signal } from '@angular/core';
import { StaffDto, StaffClient } from '../web-api-client';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export type AsyncState = 'idle' | 'loading' | 'success' | 'error';

@Injectable({ providedIn: 'root' })
export class StaffStore {
  private readonly staffClient = inject(StaffClient);

  private readonly state = signal<AsyncState>('idle');
  private readonly data = signal<StaffDto[]>([]);

  readonly staff = this.data.asReadonly();
  readonly status = this.state.asReadonly();
  readonly isLoading = computed(() => this.state() === 'loading');
  readonly isLoaded = computed(() => this.state() === 'success');

  load(branchId: number): Observable<StaffDto[]> {
    this.state.set('loading');
    return this.staffClient.getBranchStaff(branchId).pipe(
      tap({
        next: items => { this.data.set(items ?? []); this.state.set('success'); },
        error: () => this.state.set('error')
      })
    );
  }

  setStaff(items: StaffDto[]): void {
    this.data.set(items);
    this.state.set('success');
  }

  reset(): void {
    this.state.set('idle');
    this.data.set([]);
  }
}

import { Injectable, inject } from '@angular/core';
import { CanActivate, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { switchMap, take, tap, map, catchError } from 'rxjs/operators';
import { AuthService } from '../api-authorization/auth.service';
import { CenterStore } from './stores/center.store';

@Injectable({ providedIn: 'root' })
export class ProviderGuard implements CanActivate {
  private readonly authService = inject(AuthService);
  private readonly centerStore = inject(CenterStore);
  private readonly router = inject(Router);

  canActivate(_route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.authService.isAuthenticated$.pipe(
      take(1),
      switchMap(isAuthenticated => {
        if (!isAuthenticated) {
          this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
          return of(false);
        }

        if (!this.authService.hasRole('Provider')) {
          this.router.navigate(['/']);
          return of(false);
        }

        if (this.centerStore.center()) {
          return of(true);
        }

        return this.centerStore.load().pipe(
          map(() => true),
          catchError(() => {
            this.router.navigate(['/']);
            return of(false);
          })
        );
      })
    );
  }
}

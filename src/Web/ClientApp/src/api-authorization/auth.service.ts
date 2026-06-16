import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { AuthClient, LoginCommand, RegisterCommand, AuthResult } from '../app/web-api-client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _isAuthenticated = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this._isAuthenticated.asObservable();

  constructor(private authClient: AuthClient) {}

  initialize(): Observable<boolean> {
    const stored = localStorage.getItem('access_token');
    if (!stored) {
      this._isAuthenticated.next(false);
      return of(false);
    }
    return of(true).pipe(
      tap(() => this._isAuthenticated.next(true))
    );
  }

  login(email: string, password: string): Observable<AuthResult> {
    return this.authClient.login(new LoginCommand({ email, password })).pipe(
      tap(result => {
        if (result.token) {
          localStorage.setItem('access_token', result.token);
          localStorage.setItem('refresh_token', result.refreshToken ?? '');
          this._isAuthenticated.next(true);
        }
      })
    );
  }

  register(email: string, password: string, name: string, phone?: string): Observable<AuthResult> {
    return this.authClient.register(new RegisterCommand({ email, password, name, phone }));
  }

  logout(): Observable<void> {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this._isAuthenticated.next(false);
    return of(void 0);
  }
}
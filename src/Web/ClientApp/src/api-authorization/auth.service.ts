import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { AuthClient, LoginCommand, RegisterCommand } from '../app/web-api-client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private static readonly TOKEN_STORAGE_KEY = 'authToken';
  private _isAuthenticated = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this._isAuthenticated.asObservable();

  constructor(private authClient: AuthClient) {}

  initialize(): Observable<boolean> {
    const token = this.getToken();
    const isAuthenticated = !!token && !this.isTokenExpired(token);
    this._isAuthenticated.next(isAuthenticated);
    return of(isAuthenticated);
  }

  login(phone: string, password: string): Observable<void> {
    return this.authClient.login(new LoginCommand({ phone, password })).pipe(
      tap(result => this.setToken(result.token)),
      tap(() => this._isAuthenticated.next(true)),
      map(() => void 0)
    );
  }

  register(phone: string, name: string, email: string, password: string, role: string): Observable<void> {
    return this.authClient.register(new RegisterCommand({ phone, name, email, password, role })).pipe(
      map(() => void 0)
    );
  }

  logout(): Observable<void> {
    this.clearToken();
    this._isAuthenticated.next(false);
    return of(void 0);
  }

  getToken(): string | null {
    return localStorage.getItem(AuthService.TOKEN_STORAGE_KEY);
  }

  private setToken(token: string): void {
    localStorage.setItem(AuthService.TOKEN_STORAGE_KEY, token);
  }

  private clearToken(): void {
    localStorage.removeItem(AuthService.TOKEN_STORAGE_KEY);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const [, payloadBase64] = token.split('.');
      if (!payloadBase64) return true;
      const payload = JSON.parse(atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/')));
      const exp = payload?.exp;
      if (typeof exp !== 'number') return true;
      return Date.now() >= exp * 1000;
    } catch {
      return true;
    }
  }
}
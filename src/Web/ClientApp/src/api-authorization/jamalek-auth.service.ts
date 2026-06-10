import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

export interface AuthResponse {
  token: string;
  userId: string;
  name: string;
  role: string;
}

const TOKEN_KEY = 'jamalek_token';
const USER_KEY = 'jamalek_user';

@Injectable({ providedIn: 'root' })
export class JamalekAuthService {
  private _isAuthenticated = new BehaviorSubject<boolean>(!!localStorage.getItem(TOKEN_KEY));
  isAuthenticated$ = this._isAuthenticated.asObservable();

  constructor(private http: HttpClient) {}

  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  get currentUser(): AuthResponse | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  get isProvider(): boolean {
    return this.currentUser?.role === 'Provider';
  }

  get isCustomer(): boolean {
    return this.currentUser?.role === 'Customer';
  }

  initialize(): Observable<boolean> {
    const isAuth = !!this.token;
    this._isAuthenticated.next(isAuth);
    return of(isAuth);
  }

  login(phone: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth/login', { phone, password }).pipe(
      tap(res => this.setSession(res))
    );
  }

  register(phone: string, password: string, name: string, role: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth/register', { phone, password, name, role }).pipe(
      tap(res => this.setSession(res))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._isAuthenticated.next(false);
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(USER_KEY, JSON.stringify(res));
    this._isAuthenticated.next(true);
  }
}

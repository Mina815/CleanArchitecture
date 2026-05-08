import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeInterceptor implements HttpInterceptor {
  constructor(private router: Router, private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();
    const authReq = token
      ? req.clone({
          withCredentials: true,
          setHeaders: { Authorization: `Bearer ${token}` }
        })
      : req.clone({ withCredentials: true });
    return next.handle(authReq).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse
          && error.status === 401
          && !error.url?.includes('/manage/info')
          && !this.router.url.startsWith('/login')) {
          this.router.navigate(['/login'], { queryParams: { returnUrl: window.location.pathname } });
        }
        return throwError(() => error);
      })
    );
  }
}
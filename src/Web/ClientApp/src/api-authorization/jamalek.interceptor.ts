import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { JamalekAuthService } from './jamalek-auth.service';

@Injectable({ providedIn: 'root' })
export class JamalekInterceptor implements HttpInterceptor {
  constructor(private auth: JamalekAuthService) {}

  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.auth.token;
    if (token && req.url.startsWith('/api/')) {
      req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    }
    return next.handle(req);
  }
}

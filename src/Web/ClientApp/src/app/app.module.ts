import { APP_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LucideAngularModule, Sun, Moon, Laptop, Plus, Settings, MoreHorizontal, Search, Loader, Sparkles, MapPin, Star, ChevronLeft, ChevronRight, ArrowLeft, ArrowRight, Calendar, Clock, CheckCircle, CreditCard, Users, XCircle, Eye, BarChart3, Phone, Award, MessageSquare, PenSquare } from 'lucide-angular';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ThemeToggleComponent } from './theme-toggle/theme-toggle.component';
import { CenterDetailComponent } from './center-detail/center-detail.component';
import { BookingComponent } from './booking/booking.component';
import { MyBookingsComponent } from './my-bookings/my-bookings.component';
import { ProviderDashboardComponent } from './provider-dashboard/provider-dashboard.component';
import { BranchManagementComponent } from './branch-management/branch-management.component';
import { API_BASE_URL } from './web-api-client';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { LoginComponent } from 'src/api-authorization/login/login.component';
import { RegisterComponent } from 'src/api-authorization/register/register.component';
import { AuthGuard } from 'src/api-authorization/auth.guard';
export function getApiBaseUrl(): string {
  const url = document.getElementsByTagName('base')[0].href;
  return url.endsWith('/') ? url.slice(0, -1) : url;
}

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        ThemeToggleComponent,
        LoginComponent,
        RegisterComponent,
        CenterDetailComponent,
        BookingComponent,
        MyBookingsComponent,
        ProviderDashboardComponent,
        BranchManagementComponent
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        FormsModule,
        LucideAngularModule.pick({ Sun, Moon, Laptop, Plus, Settings, MoreHorizontal, Search, Loader, Sparkles, MapPin, Star, ChevronLeft, ChevronRight, ArrowLeft, ArrowRight, Calendar, Clock, CheckCircle, CreditCard, Users, XCircle, Eye, BarChart3, Phone, Award, MessageSquare, PenSquare }),
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'centers/:id', component: CenterDetailComponent },
            { path: 'book/:centerId', component: BookingComponent, canActivate: [AuthGuard] },
            { path: 'my-bookings', component: MyBookingsComponent, canActivate: [AuthGuard] },
            { path: 'provider/dashboard', component: ProviderDashboardComponent, canActivate: [AuthGuard] },
            { path: 'provider/branches', component: BranchManagementComponent, canActivate: [AuthGuard] }
        ])
    ],
    providers: [
        { provide: APP_ID, useValue: 'ng-cli-universal' },
        { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
        { provide: API_BASE_URL, useFactory: getApiBaseUrl, deps: [] },
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule { }

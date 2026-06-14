import { APP_ID, NgModule, inject, provideAppInitializer } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LucideAngularModule, Sun, Moon, Laptop } from 'lucide-angular';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ThemeToggleComponent } from './theme-toggle/theme-toggle.component';
import { CentersComponent } from './centers/centers.component';
import { CenterDetailComponent } from './center-detail/center-detail.component';
import { MyBookingsComponent } from './my-bookings/my-bookings.component';
import { ProviderDashboardComponent } from './provider-dashboard/provider-dashboard.component';
import { BranchManagementComponent } from './branch-management/branch-management.component';
import { StaffManagementComponent } from './staff-management/staff-management.component';
import { ServicesManagementComponent } from './services-management/services-management.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { API_BASE_URL } from './web-api-client';
import { JamalekInterceptor } from 'src/api-authorization/jamalek.interceptor';
import { LoginComponent } from 'src/api-authorization/login/login.component';
import { RegisterComponent } from 'src/api-authorization/register/register.component';
import { AuthGuard } from 'src/api-authorization/auth.guard';
import { JamalekAuthService } from 'src/api-authorization/jamalek-auth.service';

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
        CentersComponent,
        CenterDetailComponent,
        MyBookingsComponent,
        ProviderDashboardComponent,
        BranchManagementComponent,
        StaffManagementComponent,
        ServicesManagementComponent,
        NotificationsComponent,
        LoginComponent,
        RegisterComponent
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        FormsModule,
        LucideAngularModule.pick({ Sun, Moon, Laptop }),
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'centers', component: CentersComponent },
            { path: 'centers/:id', component: CenterDetailComponent },
            { path: 'my-bookings', component: MyBookingsComponent, canActivate: [AuthGuard] },
            { path: 'notifications', component: NotificationsComponent, canActivate: [AuthGuard] },
            { path: 'provider', component: ProviderDashboardComponent, canActivate: [AuthGuard] },
            { path: 'provider/branches', component: BranchManagementComponent, canActivate: [AuthGuard] },
            { path: 'provider/staff', component: StaffManagementComponent, canActivate: [AuthGuard] },
            { path: 'provider/services', component: ServicesManagementComponent, canActivate: [AuthGuard] },
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent }
        ])
    ],
    providers: [
        { provide: APP_ID, useValue: 'ng-cli-universal' },
        { provide: HTTP_INTERCEPTORS, useClass: JamalekInterceptor, multi: true },
        { provide: API_BASE_URL, useFactory: getApiBaseUrl, deps: [] },
        provideAppInitializer(() => inject(JamalekAuthService).initialize()),
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule { }

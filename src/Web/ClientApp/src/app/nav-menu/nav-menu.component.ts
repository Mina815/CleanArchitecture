import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { JamalekAuthService } from 'src/api-authorization/jamalek-auth.service';

@Component({
  standalone: false,
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isAuthenticated$ = this.auth.isAuthenticated$;

  constructor(public auth: JamalekAuthService, private router: Router) {}

  logout(event: Event): void {
    event.preventDefault();
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}

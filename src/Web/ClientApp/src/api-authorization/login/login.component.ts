import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { JamalekAuthService } from '../jamalek-auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  phone = '';
  password = '';
  invalid = false;

  constructor(
    private auth: JamalekAuthService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  async login() {
    this.invalid = false;
    try {
      const res = await firstValueFrom(this.auth.login(this.phone, this.password));
      const returnUrl = this.route.snapshot.queryParams['returnUrl']
        || (res.role === 'Provider' ? '/provider' : '/centers');
      await this.router.navigateByUrl(returnUrl);
    } catch {
      this.invalid = true;
      this.cdr.detectChanges();
    }
  }
}

import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { JamalekAuthService } from '../jamalek-auth.service';
import { firstValueFrom } from 'rxjs';

const MIN_PASSWORD_LENGTH = 6;

@Component({
  standalone: false,
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  phone = '';
  name = '';
  password = '';
  role = 'Customer';
  phoneTouched = false;
  passwordTouched = false;
  error = '';

  readonly minPasswordLength = MIN_PASSWORD_LENGTH;

  get phoneValid() { return /^01\d{9}$/.test(this.phone); }
  get passwordValid() { return this.password.length >= MIN_PASSWORD_LENGTH; }

  constructor(private auth: JamalekAuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  async register() {
    this.error = '';
    this.phoneTouched = true;
    this.passwordTouched = true;
    if (!this.phoneValid || !this.passwordValid || !this.name) return;
    try {
      await firstValueFrom(this.auth.register(this.phone, this.password, this.name, this.role));
      await this.router.navigate(['/centers']);
    } catch {
      this.error = 'Registration failed. Phone may already be registered.';
      this.cdr.detectChanges();
    }
  }
}

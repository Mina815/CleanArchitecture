import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
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
  email = '';
  password = '';
  role = 'Customer';
  phoneTouched = false;
  nameTouched = false;
  emailTouched = false;
  passwordTouched = false;
  error = '';

  readonly minPasswordLength = MIN_PASSWORD_LENGTH;

  get phoneValid() { return this.phone.trim().length > 0; }
  get nameValid() { return this.name.trim().length > 0; }
  get emailValid() { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email); }
  get passwordValid() {
    return this.password.length >= MIN_PASSWORD_LENGTH
      && /[A-Z]/.test(this.password)
      && /[a-z]/.test(this.password)
      && /[0-9]/.test(this.password)
      && /[^A-Za-z0-9]/.test(this.password);
  }

  constructor(private authService: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  async register() {
    this.error = '';
    this.phoneTouched = true;
    this.nameTouched = true;
    this.emailTouched = true;
    this.passwordTouched = true;
    if (!this.phoneValid || !this.nameValid || !this.emailValid || !this.passwordValid) return;
    try {
      await firstValueFrom(this.authService.register(this.phone, this.name, this.email, this.password, this.role));
      await this.router.navigate(['/login']);
    } catch (err: unknown) {
      this.error = this.getRegistrationErrorMessage(err);
      this.cdr.detectChanges();
    }
  }

  private getRegistrationErrorMessage(err: unknown): string {
    if (typeof err !== 'object' || err === null) {
      return 'Registration failed. Please try again.';
    }

    const candidate = err as { response?: string };
    if (!candidate.response) {
      return 'Registration failed. Please try again.';
    }

    try {
      const parsed = JSON.parse(candidate.response) as { errors?: Record<string, string[]> };
      const firstError = Object.values(parsed.errors ?? {})
        .flat()
        .find(message => !!message);
      return firstError ?? 'Registration failed. Please check your data.';
    } catch {
      return 'Registration failed. Please try again.';
    }
  }
}

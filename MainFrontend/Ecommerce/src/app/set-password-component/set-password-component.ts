import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { form, FormField, required, minLength, validate } from '@angular/forms/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../services/auth.Service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

interface SetPasswordFormModel {
  newPassword: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-set-password',
  standalone: true,
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './set-password-component.html',
})
export class SetPasswordComponent {
  token = signal<string>('');
  progress = signal<boolean>(false);
  successMessage = signal<string>('');
  errorMessage = signal<string>('');
  tokenExpired = signal<boolean>(false);
  maskedEmail = signal<string>('');
  resendProgress = signal<boolean>(false);
  resendSent = signal<boolean>(false);

  model = signal<SetPasswordFormModel>({ newPassword: '', confirmPassword: '' });

  setPasswordForm = form(this.model, (path) => {
    required(path.newPassword, { message: 'Password is required' });
    minLength(path.newPassword, 8, { message: 'Password must be at least 8 characters' });

    required(path.confirmPassword, { message: 'Please confirm your password' });

    validate(path.confirmPassword, (ctx) => {
      const pwd = ctx.valueOf(path.newPassword);
      const confirm = ctx.value();
      if (pwd && confirm && pwd !== confirm) {
        return { kind: 'mismatch', message: 'Passwords do not match' };
      }
      return null;
    });
  });

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    const tokenParam = this.route.snapshot.queryParamMap.get('token') ?? '';
    this.token.set(tokenParam);

    if (!tokenParam) {
      this.errorMessage.set('No token found. Please use the link from your email.');
    }
  }

  handleSubmitClick(event: Event) {
  event.preventDefault();
  event.stopPropagation();

  this.errorMessage.set('');
  this.successMessage.set('');

  if (this.setPasswordForm().invalid()) {
    this.setPasswordForm().markAsTouched();
    return;
  }

  this.progress.set(true);

  this.authService
    .setPassword({
      token: this.token(),
      newPassword: this.setPasswordForm.newPassword().value(),
    })
    .subscribe({
      next: (res) => {
        this.progress.set(false);
        this.successMessage.set(res.message ?? 'Password set successfully.');
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.progress.set(false);
        const status = err?.status;
        const backendMessage = err?.error?.message ?? '';

        if (status === 400 && backendMessage.toLowerCase().includes('expired')) {
          this.tokenExpired.set(true);
          this.maskedEmail.set(err?.error?.maskedEmail ?? '');
          this.errorMessage.set('This link has expired. Request a new one below.');
        } else if (status === 400 && backendMessage.toLowerCase().includes('used')) {
          this.errorMessage.set('This link has already been used. Please log in or request a new invite.');
        } else {
          this.errorMessage.set(backendMessage || 'Something went wrong. Please try again.');
        }
      },
    });
}

  resendInvite() {
    if (!this.maskedEmail()) {
      this.errorMessage.set('Please contact your administrator to resend the invite.');
      return;
    }

    this.resendProgress.set(true);
    this.authService
      .resendInvite({ email: this.maskedEmail() })
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: () => {
          this.resendProgress.set(false);
          this.resendSent.set(true);
          this.errorMessage.set('');
        },
        error: (err) => {
          this.resendProgress.set(false);
          this.errorMessage.set(err?.error?.message ?? 'Failed to resend invite.');
        },
      });
  }

  resetForm() {
    this.model.set({ newPassword: '', confirmPassword: '' });
    this.errorMessage.set('');
    this.successMessage.set('');
  }
}
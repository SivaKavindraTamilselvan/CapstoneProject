import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProfileResponseModel } from '../models/profile/profile.response.model';
import { ProfileService } from '../services/user-profile.Service';
import { Router } from '@angular/router';
import { ChangePasswordModel } from '../models/profile/change.password.model';
import { DatePipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-profile-page',
  imports: [FormsModule,NgClass,DatePipe],
  templateUrl: './profile-page.html',
  styleUrl: './profile-page.css',
})
export class ProfilePage {
  profile = signal<ProfileResponseModel | null>(null);

  successMessage = signal('');
  errorMessage = signal('');

  passwordPanelOpen = signal(false);
  passwordErrorMessage = signal('');

  currentPassword = '';
  newPassword = '';
  confirmPassword = '';

  showCurrentPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);

  constructor(private profileService: ProfileService, private router: Router) { }

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.profileService.getProfile().subscribe({
      next: (response: any) => {
        this.profile.set(response);
      },
      error: () => {
        this.errorMessage.set('Failed to load profile. Please try again.');
      },
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    });
  }

  togglePasswordPanel() {
    this.passwordPanelOpen.set(!this.passwordPanelOpen());
    this.passwordErrorMessage.set('');
  }

  changePassword() {
    this.passwordErrorMessage.set('');

    if (!this.currentPassword || !this.newPassword || !this.confirmPassword) {
      this.passwordErrorMessage.set('All fields are required.');
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.passwordErrorMessage.set('New password and confirm password do not match.');
      return;
    }

    if (this.newPassword.length < 8) {
      this.passwordErrorMessage.set('New password must be at least 8 characters.');
      return;
    }

    this.profileService
      .changePassword(
        new ChangePasswordModel(
          this.currentPassword,
          this.newPassword
        )
      )
      .subscribe({
        next: () => {
          this.successMessage.set('Password updated successfully.');
          this.cancelPasswordChange();
          setTimeout(() => this.successMessage.set(''), 4000);
        },
        error: (err: any) => {
          this.passwordErrorMessage.set(
            err?.error?.message ??
            'Failed to update password. Please check your current password.'
          );
        },
      });

  }

  cancelPasswordChange() {
    this.currentPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';
    this.passwordPanelOpen.set(false);
    this.passwordErrorMessage.set('');
  }
}
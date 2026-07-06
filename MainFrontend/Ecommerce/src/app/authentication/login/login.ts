import { Component, signal } from '@angular/core';
import { FormField, form, required, email } from '@angular/forms/signals';
import { LoginModel } from '../../models/authentication/login.model';
import { AuthService } from '../../services/auth.Service';
import { Router, RouterLink } from '@angular/router';
import { AuthStateService } from '../../services/auth-State.Service';
import { FormsModule } from '@angular/forms';
import { ROLES } from '../../constant/role.constant';

@Component({
  selector: 'app-login',
  imports: [FormField, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  errorMessage = signal<string | null>(null);
  loginModel = signal(new LoginModel());
  progress = signal(false);
  constructor(private authService: AuthService, private router: Router, private authStateService: AuthStateService) {

  }

  loginForm = form(this.loginModel, (path) => {
    required(path.password, { message: "Password Is Required" });
    required(path.email, { message: "Email Is Required" });
    email(path.email, { message: "Enter Valid Email Address" });
  })

  handleLoginClick() {
    this.errorMessage.set(null);
    if (this.loginForm().invalid()) {
      this.errorMessage.set("Enter Proper Details");
      return;
    }
    this.progress.set(true);
    this.authService.loginAPICall(this.loginModel()).subscribe({
      next: (response: any) => {
        this.authStateService.login(response.token);
        const roleId = this.authStateService.getRoleId();

        if (roleId === ROLES.ADMIN) {
          this.router.navigate(['/admin']);
        }
        else if (roleId === ROLES.VENDOR) {
          this.router.navigate(['/vendor']);
        }
        else if (roleId === ROLES.USER) {
          this.router.navigate(['/']);
        }
        else {
          this.router.navigate(['/user/product']);
        }
        this.progress.set(false);
      },
      error: (error) => {
        this.progress.set(false);
        this.errorMessage.set(null);

        if (error.status === 401) {
          this.errorMessage.set("Invalid email or password");
        }
        else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set("Something went wrong. Please try again.");
        }
      }
    })
  }
}

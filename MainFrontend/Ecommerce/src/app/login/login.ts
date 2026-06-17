import { Component, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormField, form, required, email } from '@angular/forms/signals';
import { LoginModel } from '../models/login.model';
import { AuthService } from '../services/auth.Service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule, FormField],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  errorMessage = signal<string | null>(null);
  loginModel = signal(new LoginModel());
  progress = signal(false);
  constructor(private authService: AuthService, private router: Router) {

  }
  loginForm = form(this.loginModel, (path) => {
    required(path.password, { message: "Password Is Required" });
    required(path.email, { message: "Email Is Required" });
    email(path.email, { message: "Enter Valid Email Address" });
  })
  handleLoginClick() {
    if (this.loginForm().invalid()) {
      alert("Please fix the errors in the form before submitting");
      return;
    }
    this.progress.set(true);
    this.authService.loginAPICall(this.loginModel()).subscribe({
      next: (response: any) => {
        console.log("Login Successfull", response);
        sessionStorage.setItem("token", response.token);
        alert("Login successful!")
        this.progress.set(false);
      },
      error: (error) => {
        console.error(error);
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

import { Component, signal } from '@angular/core';
import { RegisterModel } from '../models/authentication/register-user.model';
import { AuthService } from '../services/auth.Service';
import { Router } from '@angular/router';
import { FormField, email, form, pattern, required } from '@angular/forms/signals';

@Component({
  selector: 'app-register',
  imports: [FormField],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  errorMessage = signal<string | null>(null);
  registerModel = signal(new RegisterModel());
  progress = signal(false);
  showPassword = signal(false);
  constructor(private router: Router, private authService: AuthService) {

  }
  togglePasswordVisibility() {
    this.showPassword.update(v => !v);
  }

  registerForm = form(this.registerModel, (path) => {
    required(path.email, { message: "Enter The Email" })
    required(path.firstName, { message: "Enter The FirstName" })
    required(path.lastName, { message: "Enter The Last Name" })
    required(path.phoneNumber, { message: "Enter The Phone Number" })
    required(path.password, { message: "Enter The Password" })
    email(path.email, { message: "Enter Valid Email Address" })
    pattern(path.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: "Enter Valid Phone Number" })
    pattern(path.password, /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,12}$/, { message: "Password must contains 8-12 characters,uppercase,lowercase,symbols and number" })
  })
  handleRegisterClick() {
    if (this.registerForm().invalid()) {
      alert("Enter Proper Details");
      return;
    }
    this.errorMessage.set(null);
    this.progress.set(true);
    this.authService.registerUserAPICall(this.registerModel()).subscribe({
      next: (response: any) => {
        alert("Registration Successfull");
        this.progress.set(false);
      },
      error: (error) => {
        console.error(error);
        this.progress.set(false);
        this.errorMessage.set(null);
        if (error.status == 409) {
          this.errorMessage.set(error.error.message)
        }
        else if (error.status == 401) {
          this.errorMessage.set("Invalid Email or Password");
        }
        else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set("Something went wrong. Please try again.")
        }
      }
    })
  }
  resetFilter() {
    this.registerModel.set(new RegisterModel());
    this.errorMessage.set(null);
  }
}
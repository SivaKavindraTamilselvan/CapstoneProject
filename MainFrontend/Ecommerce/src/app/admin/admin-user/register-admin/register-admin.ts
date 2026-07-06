import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.Service';
import { RegisterAdminModel } from '../../../models/authentication/register-admin.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormField, email, form, pattern, required } from '@angular/forms/signals';

@Component({
  selector: 'app-register-admin',
  imports: [FormsModule, FormField, ReactiveFormsModule],
  templateUrl: './register-admin.html',
  styleUrl: './register-admin.css',
})
export class RegisterAdmin {
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  adminModel = signal(new RegisterAdminModel());
  progress = signal(false);
  constructor(private router: Router, private authService: AuthService) {

  }
  registerForm = form(this.adminModel, (path) => {
    required(path.requestRegisterUserDTO.email, { message: "Enter The Email" })
    required(path.requestRegisterUserDTO.firstName, { message: "Enter The FirstName" })
    required(path.requestRegisterUserDTO.lastName, { message: "Enter The Last Name" })
    required(path.requestRegisterUserDTO.phoneNumber, { message: "Enter The Phone Number" })
    required(path.requestRegisterUserDTO.password, { message: "Enter The Password" })
    email(path.requestRegisterUserDTO.email, { message: "Enter Valid Email Address" })
    pattern(path.requestRegisterUserDTO.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: "Enter Valid Phone Number" })
    pattern(path.requestRegisterUserDTO.password, /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,12}$/, { message: "Password must contains 8-12 characters,uppercase,lowercase,symbols and number" })
    required(path.adminRoleId, { message: "Admin Role Id Is Required" })
    pattern(path.adminRoleId, /^[2-9]{1}$/, { message: "Select a valid admin role" })
  })
  handleRegisterClick(event : Event) {
    event.preventDefault();
    this.errorMessage.set(null);
    this.successMessage.set(null);
    if (this.registerForm().invalid()) {
      this.errorMessage.set("Enter Proper Details");
      return;
    }
    this.progress.set(true);
    this.authService.registerAdminAPICall(this.adminModel()).subscribe({
      next: (response: any) => {
        this.successMessage.set("Registration Successfull");
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
    this.adminModel.set(new RegisterAdminModel());
    this.registerForm().reset();
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }
}

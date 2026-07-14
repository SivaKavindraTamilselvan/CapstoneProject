import { Component, signal } from '@angular/core';
import { RegisterVendorUserModel } from '../../../models/authentication/register-vendor-user.model';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.Service';
import { email, form, FormField, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-register-vendor',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './register-vendor.html',
  styleUrl: './register-vendor.css',
})
export class RegisterVendor {
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  vendorModel = signal(new RegisterVendorUserModel());
  progress = signal(false);
  constructor(private router: Router, private authService: AuthService) {

  }
  registerForm = form(this.vendorModel, (path) => {
    required(path.requestRegisterUserDTO.email, { message: "Enter The Email" })
    required(path.requestRegisterUserDTO.firstName, { message: "Enter The FirstName" })
    required(path.requestRegisterUserDTO.lastName, { message: "Enter The Last Name" })
    required(path.requestRegisterUserDTO.phoneNumber, { message: "Enter The Phone Number" })
    email(path.requestRegisterUserDTO.email, { message: "Enter Valid Email Address" })
    pattern(path.requestRegisterUserDTO.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: "Enter Valid Phone Number" })
    required(path.vendorRoleId, { message: "Vendor Role Id Is Required" })
    pattern(path.vendorRoleId, /^[2-9]{1}$/, { message: "Select a valid vendor role" })
  })

  scrollToTop(): void {
    window.scrollTo({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

  handleRegisterClick() {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    if (this.registerForm().invalid()) {
      this.errorMessage.set("Enter Proper Details");
      this.scrollToTop();
      return;
    }
    this.progress.set(true);
    this.vendorModel().requestRegisterUserDTO.password = "Vendor@123";
    this.authService.registerVendorUserAPICall(this.vendorModel()).subscribe({
      next: (response: any) => {
        this.successMessage.set("Registration Successfull");
        this.progress.set(false);
        this.scrollToTop();
      },
      error: (error) => {
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
        this.scrollToTop();
      }
    })
  }
  resetFilter() {
    this.vendorModel.set(new RegisterVendorUserModel());
    this.registerForm().reset();

    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.scrollToTop();
  }
}


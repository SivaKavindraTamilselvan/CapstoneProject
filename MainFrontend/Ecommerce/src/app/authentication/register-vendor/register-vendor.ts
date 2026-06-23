import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.Service';
import { RegisterVendorModel } from '../../models/authentication/regiser-vendor.model';
import { FormField, email, form, pattern, required } from '@angular/forms/signals';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register-vendor',
  imports: [FormField, RouterLink, FormsModule],
  templateUrl: './register-vendor.html',
  styleUrl: './register-vendor.css',
})
export class RegisterVendor {

  registerModel = signal(new RegisterVendorModel());
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  progress = signal(false);
  showPassword = signal(false);
  constructor(private router: Router, private authService: AuthService) {

  }
  registerForm = form(this.registerModel, (path) => {
    required(path.requestRegisterUserDTO.email, { message: "Enter The Email" })
    required(path.requestRegisterUserDTO.firstName, { message: "Enter The FirstName" })
    required(path.requestRegisterUserDTO.lastName, { message: "Enter The Last Name" })
    required(path.requestRegisterUserDTO.phoneNumber, { message: "Enter The Phone Number" })
    required(path.requestRegisterUserDTO.password, { message: "Enter The Password" })
    email(path.requestRegisterUserDTO.email, { message: "Enter Valid Email Address" })
    pattern(path.requestRegisterUserDTO.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: "Enter Valid Phone Number" })
    pattern(path.requestRegisterUserDTO.password, /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,12}$/, { message: "Password must contains 8-12 characters,uppercase,lowercase,symbols and number" })
    required(path.contactPersonName, { message: "Enter The Contact Person Name" })
    required(path.vendorCompanyName, { message: "Enter The Company Name" })
    required(path.companyEmail, { message: "Enter The Company Email" })
    required(path.gstNumber, { message: "Enter The GST Number" })
    email(path.companyEmail, { message: "Enter Valid Email Address" })
    pattern(path.companyPhoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: "Enter Valid Phone Number" })
  })
  handleRegisterClick() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.registerForm().invalid()) {
      this.errorMessage.set("Enter Proper Details");
      return;
    }
    this.progress.set(true);
    this.authService.registerVendorAPICall(this.registerModel()).subscribe({
      next: (response: any) => {
        this.successMessage.set("Vendor registration submitted successfully. Please wait for admin approval.");
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
  togglePasswordVisibility() {
    this.showPassword.update(v => !v);
  }

  resetFilter() {
    this.registerModel.set(new RegisterVendorModel());
    this.errorMessage.set(null);
  }
}
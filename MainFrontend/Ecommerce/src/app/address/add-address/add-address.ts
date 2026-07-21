import { Component, signal } from '@angular/core';
import { AddressService } from '../../services/address.Service';
import { AddAddressModel } from '../../models/address/add-address.model';
import { form, FormField, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { BaseURL } from '../../environment';

interface PostOffice {
  Name: string;
  District: string;
  State: string;
  Pincode: string;
}

interface PinApiResponse {
  Status: string;
  PostOffice: PostOffice[] | null;
}

@Component({
  selector: 'app-add-address',
  imports: [ReactiveFormsModule, FormsModule, FormField],
  templateUrl: './add-address.html',
  styleUrl: './add-address.css',
})
export class AddAddress {
  address = signal(new AddAddressModel());

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  progress = signal(false);
  submitted = signal(false);

  pinLookupLoading = signal(false);
  pinLookupError = signal<string | null>(null);
  cityOptions = signal<string[]>([]);

  constructor(
    private addressService: AddressService,
    private http: HttpClient
  ) { }

  addForm = form(this.address, (path) => {
    required(path.contactName, {
      message: 'Contact name is required',
    });

    required(path.contactPhoneNumber, {
      message: 'Contact phone number is required',
    });

    pattern(path.contactPhoneNumber, /^[6-9]\d{9}$/, {
      message: 'Enter a valid 10-digit Indian phone number',
    });

    required(path.addressLine, {
      message: 'Address line is required',
    });

    required(path.landMark, {
      message: 'Landmark is required',
    });

    required(path.city, {
      message: 'City is required',
    });

    required(path.state, {
      message: 'State is required',
    });

    required(path.pinCode, {
      message: 'Pin code is required',
    });

    pattern(path.pinCode, /^[1-9][0-9]{5}$/, {
      message: 'Enter a valid 6-digit pin code',
    });
  });

  onPinCodeInput(event: Event) {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const input = event.target as HTMLInputElement;
    const pin = input.value.replace(/\D/g, '').slice(0, 6);

    input.value = pin;

    this.addForm.pinCode().markAsTouched();

    this.address.update((a) => ({
      ...a,
      pinCode: pin,
      city: '',
      state: '',
    }));

    this.cityOptions.set([]);
    this.pinLookupError.set(null);

    if (/^[1-9][0-9]{5}$/.test(pin)) {
      this.fetchPinData(pin);
    } else if (pin.length === 6) {
      this.pinLookupError.set('Enter a valid PIN code. PIN cannot start with 0.');
    }
  }
  fetchPinData(pin: string) {
    this.pinLookupLoading.set(true);
    this.pinLookupError.set(null);
    this.http.get<PinApiResponse[]>(`${BaseURL}/Address/pincode/${pin}`).subscribe({
      next: (response) => {
        this.pinLookupLoading.set(false);
        const result = response?.[0];
        if (result?.Status === 'Success' && result.PostOffice?.length) {
          const postOffices = result.PostOffice;
          const cities = [...new Set(postOffices.map((po) => po.Name))];
          const state = postOffices[0].State;
          this.cityOptions.set(cities);
          this.address.update((a) => ({
            ...a,
            state: state,
            city: '',
          }));
          this.addForm.state().markAsTouched();
        }
        else {
          this.pinLookupError.set('No results found for this PIN code.');
          this.cityOptions.set([]);
          this.address.update((a) => ({
            ...a,
            city: '',
            state: '',
          }));
        }
      },
      error: () => {
        this.pinLookupLoading.set(false);
        this.pinLookupError.set('Failed to fetch PIN code data. Check your connection.');
      },
    });
  }

  onCityChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.addForm.city().markAsTouched();
    this.address.update((a) => ({
      ...a,
      city: select.value,
    }));
  }
  onIsDefaultChange(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    this.address.update((a) => ({
      ...a,
      isDefault: checkbox.checked,
    }));
  }
  touchAllFields() {
    this.addForm.contactName().markAsTouched();
    this.addForm.contactPhoneNumber().markAsTouched();
    this.addForm.addressLine().markAsTouched();
    this.addForm.landMark().markAsTouched();
    this.addForm.city().markAsTouched();
    this.addForm.state().markAsTouched();
    this.addForm.pinCode().markAsTouched();
  }
  addAddress() {
    this.submitted.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.touchAllFields();
    if (this.addForm().invalid()) {
      this.errorMessage.set('Please fill in all required fields correctly.');
      return;
    }
    this.progress.set(true);
    this.addressService.addAddress(this.address()).subscribe({
      next: () => {
        this.progress.set(false);
        this.resetForm();
        this.submitted.set(false);
        this.successMessage.set('Address added successfully!');
      },
      error: (error) => {
        this.progress.set(false);
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(', ');

          this.errorMessage.set(messages);
        }
        else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        }
        else if (error.status === 0) {
          this.errorMessage.set('Unable to connect to the server. Please check your internet connection.');
        }
        else if (error.status >= 500) {
          this.errorMessage.set('Something went wrong on the server. Please try again later.');
        }
        else {
          this.errorMessage.set('Failed to add address.');
        }
      },
    });
  }
  resetForm() {
    this.address.set(new AddAddressModel());
    this.cityOptions.set([]);
    this.pinLookupError.set(null);
    this.addForm().reset();
    this.submitted.set(false);
    this.pinLookupLoading.set(false);
  }
  resetFilter() {
    this.resetForm();
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }
}
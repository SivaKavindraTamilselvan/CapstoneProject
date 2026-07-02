import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorUserModel } from '../../../models/vendor/vendor-user/response-vendor-user.model';
import { VendorUserFilter } from '../../../models/vendor/vendor-user/vendor-user.filter';
import { email, form, pattern } from '@angular/forms/signals';
import { Router } from '@angular/router';
import { VendorUserService } from '../../../services/vendor-user.Service';

@Component({
  selector: 'app-deactive-vendor-user',
  imports: [],
  templateUrl: './deactive-vendor-user.html',
  styleUrl: './deactive-vendor-user.css',
})
export class DeactiveVendorUser {
  adminUsers = signal<PagedResponse<VendorUserModel> | null>(null);
  showActivatePopup = signal(false);
  selectedAdminId = signal<number | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);
  adminRoleId = signal<number | null>(null);
  filterPanelOpen = signal<boolean>(false);

  progress = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  filtererrorMessage = signal<string | null>(null);
  filterapplied = signal(false);

  adminUserFilter = signal(new VendorUserFilter());

  filterForm = form(this.adminUserFilter, (path) => {
    email(path.email, { message: 'Enter a valid email address.' });
    pattern(path.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: 'Enter a valid phone number.' });
  });

  constructor(private route: Router, private adimUserService: VendorUserService) {

  }
  ngOnInit(): void {
    this.loadDeactiveAdminUser();
  }
  loadDeactiveAdminUser() {
    this.adimUserService.getDeactiveAdminUser(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.adminUsers.set(response);
      },
      error: (error) => {
        console.error(error);
        this.progress.set(false);
        this.errorMessage.set(null);
        if (error.status === 404) {
          this.adminUsers.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
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
  activateAdmin() {
    const id = this.selectedAdminId();
    if (id == null) {
      return;
    }
    this.adimUserService.activateAdminUser(id).subscribe({
      next: (response: any) => {
        this.closePopup();
        this.loadDeactiveAdminUser();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  confirmActivate(id: number) {
    this.selectedAdminId.set(id);
    this.showActivatePopup.set(true);
  }
  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedAdminId.set(null);
  }
  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.pageNumber.set(1);
    this.pageSize.set(value);
    this.loadDeactiveAdminUser();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadDeactiveAdminUser();
  }
  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }
  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }
  adminRoleOption = [
    { id: 1, label: 'Overall Admin' },
    { id: 2, label: 'Vendor Admin' },
    { id: 3, label: 'Product Admin' },
    { id: 4, label: 'Order Admin' },
    { id: 5, label: 'Coupons Logistic Admin' },
    { id: 6, label: 'Return Admin' },
    { id: 7, label: 'Refund Admin' },
    { id: 8, label: 'Exchange Admin' },
    { id: 9, label: 'Payment Admin' }
  ]
  onAdminRoleChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.adminRoleId.set(v ? Number(v) : null);
  }
 toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }
   closeFilterPanel(): void {
     if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.filterPanelOpen.set(false);

  }

  applyFilter(): void {
    if (this.filterForm().invalid()) {
      return;
    }
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: 1 }));
    this.loadDeactiveAdminUser();
    this.closeFilterPanel();
  }
 resetFilter(): void {
    this.adminUserFilter.set(new VendorUserFilter());
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: 1 }));
    this.loadDeactiveAdminUser();
    this.filterapplied.set(false);
  }
  private buildFilter(): VendorUserFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      vendorRoleId: this.adminRoleId(),
      status: false,
      email: '',
      phoneNumber: ''
    };
  }
  onPhoneNumberChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim();
    this.adminUserFilter.update(filter => ({ ...filter, phoneNumber: value }));
  }

  onEmailChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.adminUserFilter.update(filter => ({ ...filter, email: value }));
  }
}


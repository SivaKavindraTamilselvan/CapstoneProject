import { Component, signal, computed } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { Router } from '@angular/router';
import { AdminVendorFilter } from '../../../models/admin/vendor/admin-vendor.filter';

@Component({
  selector: 'app-delete-vendor',
  imports: [],
  templateUrl: './delete-vendor.html',
  styleUrl: './delete-vendor.css',
})
export class DeleteVendor {
  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.vendors()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);
  contactPersonName = signal<string>("");
  companyEmail = signal<string>("");
  companyPhoneNumber = signal<string>("");
  vendorCompanyName = signal<string>("");
  gstNumber = signal<string>("");
  approvalStatusId = signal<number | null>(null);
  isActive = signal<boolean | null>(null);
  reviewedByAdminId = signal<number | null>(null);
  filtererrorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(private route: Router, private adminVendorService: AdminVendorService) {

  }
  ngOnInit(): void {
    this.loadVendor();
  }
  loadVendor() {
    this.adminVendorService.getDeletedVendor(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.vendors.set(response);
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.vendors.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
          });
        }
        else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(error.errorMessage);
        }
      }
    })
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadVendor();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadVendor();
  }
  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }
  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }
  toggleFilterPanel(): void {
    const wasOpen = this.filterPanelOpen();
    this.filterPanelOpen.update((open) => !open);
    if (wasOpen && !this.filterapplied()) {
      this.resetFilter();
    }
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  applyFilter(): void {
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadVendor();
    this.closeFilterPanel();
  }
  resetFilter(): void {
    this.filtererrorMessage.set("");
    this.filterapplied.set(false);
    this.isActive.set(null);
    this.approvalStatusId.set(null);
    this.companyEmail.set("");
    this.contactPersonName.set("");
    this.companyPhoneNumber.set("");
    this.gstNumber.set("");
    this.vendorCompanyName.set("");
    this.reviewedByAdminId.set(null);
    this.pageNumber.set(1);
    this.loadVendor();
  }
  private buildFilter(): AdminVendorFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: this.isActive(),
      contactPersonName: this.contactPersonName(),
      companyEmail: this.companyEmail().trim(),
      companyPhoneNumber: this.companyPhoneNumber(),
      vendorCompanyName: this.vendorCompanyName().trim(),
      gstNumber: this.gstNumber().trim(),
      approvalStatusId: this.approvalStatusId(),
      reviewedByAdminId: this.reviewedByAdminId()
    };
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.isActive.set(null);
    }
    else {
      this.isActive.set(value === 'true');
    }
  }
  onApprovalChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.approvalStatusId.set(v ? Number(v) : null);
  }
  onReviewedAdminChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.reviewedByAdminId.set(v ? Number(v) : null);
  }
  onVendorCompanyNameInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.vendorCompanyName.set(v);
  }
  onVendorCompanyEmailInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim();

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    this.companyEmail.set(value);
    if (value === "" || emailRegex.test(value)) {
      this.filtererrorMessage.set(null);
    }
    else {
      this.filtererrorMessage.set("Enter a valid email address.");
    }
  }
  onContactPersonInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.contactPersonName.set(v);
  }
  onVendorCompanyPhoneNumberInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim();

    const phoneRegex = /^\d{0,10}$/;
    this.companyPhoneNumber.set(value);
    if (phoneRegex.test(value)) {
      this.filtererrorMessage.set(null);
    }
    else {
      this.filtererrorMessage.set("Phone number must contain only digits and be at most 10 digits.");
    }
  }
  onGstNumberInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim().toUpperCase();

    const gstRegex = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$/;
    this.gstNumber.set(value);
    if (value === "" || gstRegex.test(value)) {
      this.filtererrorMessage.set(null);
    }
    else {
      this.filtererrorMessage.set("Enter a valid GST number.");
    }
  }
}

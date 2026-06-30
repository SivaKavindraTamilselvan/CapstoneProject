import { Component, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { AdminVendorFilter } from '../../../models/admin/vendor/admin-vendor.filter';
import { AdminDeleteVendorModel } from '../../../models/admin/vendor/delete-vendor.model';
import { form, FormField, maxLength, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-vendor-list',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './vendor-list.html',
  styleUrl: './vendor-list.css',
})
export class VendorList {
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
  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteVendorModel = signal(new AdminDeleteVendorModel());
  selectedVendorId = signal<number | null>(null);
  filtererrorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);

  constructor(private route: Router, private adminVendorService: AdminVendorService) {

  }
  ngOnInit(): void {
    this.loadVendor();
  }
  loadVendor() {
    this.adminVendorService.getVendor(this.buildFilter()).subscribe({
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
  deleteForm = form(this.deleteVendorModel, (path) => {
    required(path.remark, { message: "Enter The Remarks" });
    maxLength(path.remark, 150, { message: "Maximum 100 characters" });
  })

  handleDelete() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const errors = [];

    if (this.deleteForm.remark().invalid()) {
      errors.push(this.deleteForm.remark().errors()[0].message);
    }
    this.errorMessage.set(errors.join(", "));

    if (this.deleteForm().invalid()) {
      return;
    }
    this.progress.set(true);

    const request = {
      vendorId: this.deleteVendorModel().vendorId,
      remark: this.deleteVendorModel().remark
    };

    this.adminVendorService.DeleteVendor(request).subscribe({
      next: () => {
        this.successMessage.set("Vendor deleted successfully");

        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadVendor();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
        this.progress.set(false);
      }
    });
  }

  openDeletePopup(vendorId: number) {
    this.selectedVendorId.set(vendorId);

    this.deleteVendorModel.update(model => ({
      ...model,
      vendorId: vendorId,
      remark: ''
    }));

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedVendorId.set(null);
    this.deleteVendorModel.set(new AdminDeleteVendorModel());
    this.errorMessage.set(null);
  }
  approvalStatusOption = [
    { id: 2, label: 'Accepted' },
    { id: 3, label: 'Rejected' },
  ]
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
  viewVendor(vendorId: number) {
    this.route.navigate(['/admin/vendors', vendorId]);
  }
}

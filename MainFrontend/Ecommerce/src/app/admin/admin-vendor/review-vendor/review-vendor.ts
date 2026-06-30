import { Component, signal, computed } from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { ReviewVendorModel } from '../../../models/admin/vendor/review.vendor.model';
import { FormField, form, maxLength, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AdminVendorFilter } from '../../../models/admin/vendor/admin-vendor.filter';

@Component({
  selector: 'app-review-vendor',
  imports: [FormField, ReactiveFormsModule, FormsModule],
  templateUrl: './review-vendor.html',
  styleUrl: './review-vendor.css',
})
export class ReviewVendor {
  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);
  showActivatePopup = signal(false);
  selectedVendorId = signal<number | null>(null);
  reviewVendorModel = signal(new ReviewVendorModel());
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
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  filtererrorMessage = signal<string | null>(null);
  progress = signal(false);
  filterapplied = signal(false);
  constructor(private route: Router, private adminVendorService: AdminVendorService) {

  }

  ngOnInit(): void {
    this.loadPendingVendor();
  }

  reviewForm = form(this.reviewVendorModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[23]$/, { message: "Select valid approval status" });
    maxLength(path.remark, 150, { message: "Maximum 100 characters" });
  })

  loadPendingVendor() {
    this.adminVendorService.getPendingVendor(this.buildFilter()).subscribe({
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
  openReviewPopup(vendorId: number) {
    this.selectedVendorId.set(vendorId);

    this.reviewVendorModel.update(model => ({
      ...model,
      vendorId: vendorId,
      approvalStatusId: '',
      remark: ''
    }));

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedVendorId.set(null);
    this.reviewVendorModel.set(new ReviewVendorModel());
    this.errorMessage.set(null);
  }

  handleReview() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const errors = [];

    if (this.reviewForm.approvalStatusId().invalid()) {
      errors.push(this.reviewForm.approvalStatusId().errors()[0].message);
    }

    if (this.reviewForm.remark().invalid()) {
      errors.push(this.reviewForm.remark().errors()[0].message);
    }

    this.errorMessage.set(errors.join(", "));

    if (this.reviewForm().invalid()) {
      return;
    }
    this.progress.set(true);

    const request = {
      vendorId: this.reviewVendorModel().vendorId,
      approvalStatusId: Number(this.reviewVendorModel().approvalStatusId),
      remark: this.reviewVendorModel().remark.trim()
    };

    this.adminVendorService.reviewVendor(request).subscribe({
      next: () => {
        this.successMessage.set("Vendor reviewed successfully.Closing in 3 seconds...");
        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadPendingVendor();
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

  approvalStatusOption = [
    { id: 2, label: 'Accepted' },
    { id: 3, label: 'Rejected' },
  ]
  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadPendingVendor();
  }
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.pageNumber.set(page);
    this.loadPendingVendor();
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
    if(this.filtererrorMessage()){
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadPendingVendor();
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
    this.loadPendingVendor();
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

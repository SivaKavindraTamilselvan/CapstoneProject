import { Component, signal ,computed} from '@angular/core';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { ReviewVendorModel } from '../../../models/admin/vendor/review.vendor.model';
import { FormField, form, pattern, required } from '@angular/forms/signals';
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
  constructor(private route: Router, private adminVendorService: AdminVendorService) {

  }

  ngOnInit(): void {
    this.loadPendingVendor();
  }

  reviewForm = form(this.reviewVendorModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    pattern(path.approvalStatusId, /^[23]$/, { message: "Select valid approval status" })
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
        else {
          console.log(error);
        }
      }
    })
  }
  openReviewPopup(vendorId: number) {
    this.selectedVendorId.set(vendorId);

    this.reviewVendorModel.set(
      new ReviewVendorModel(vendorId, "", "")
    );

    this.showActivatePopup.set(true);
  }

  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedVendorId.set(null);
    this.reviewVendorModel.set(new ReviewVendorModel());
  }
  handleReview() {
    if (this.reviewForm().invalid()) {
      alert("Enter proper details");
      return;
    }

    const request = {
      vendorId: this.reviewVendorModel().vendorId,
      approvalStatusId: Number(this.reviewVendorModel().approvalStatusId),
      remark: this.reviewVendorModel().remark
    };

    this.adminVendorService.reviewVendor(request).subscribe({
      next: () => {
        alert("Vendor reviewed successfully");
        this.closePopup();
        this.loadPendingVendor();
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
  approvalStatusOption = [
      { id: 1, label: 'Pending' },
      { id: 2, label: 'Accepted' },
      { id: 3, label: 'Rejected' },
      { id: 4, label: 'Deleted By Admin' }
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
      this.filterPanelOpen.update((open) => !open);
    }
    closeFilterPanel(): void {
      this.filterPanelOpen.set(false);
    }
    applyFilter(): void {
      this.pageNumber.set(1);
      this.loadPendingVendor();
      this.closeFilterPanel();
    }
    resetFilter(): void {
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
        companyEmail: this.companyEmail(),
        companyPhoneNumber: this.companyPhoneNumber(),
        vendorCompanyName: this.vendorCompanyName(),
        gstNumber: this.gstNumber(),
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
      const v = (event.target as HTMLInputElement).value;
      this.companyEmail.set(v);
    }
    onContactPersonInput(event: Event): void {
      const v = (event.target as HTMLInputElement).value;
      this.contactPersonName.set(v);
    }
    onVendorCompanyPhoneNumberInput(event: Event): void {
      const v = (event.target as HTMLInputElement).value;
      this.companyPhoneNumber.set(v);
    }
    onGstNumberInput(event: Event): void {
      const v = (event.target as HTMLInputElement).value;
      this.gstNumber.set(v);
    }
}

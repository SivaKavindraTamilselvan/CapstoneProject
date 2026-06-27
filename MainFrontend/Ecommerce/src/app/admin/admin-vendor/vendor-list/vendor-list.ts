import { Component, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { AdminVendorFilter } from '../../../models/admin/vendor/admin-vendor.filter';
import { AdminDeleteVendorModel } from '../../../models/admin/vendor/delete-vendor.model';
import { form, FormField, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-vendor-list',
  imports: [FormField,ReactiveFormsModule,FormsModule],
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
      }
    })
  }
  deleteForm = form(this.deleteVendorModel, (path) => {
    required(path.remark, { message: "Enter The Remarks" });
  })

  handleDelete() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.deleteForm().invalid()) {
      this.errorMessage.set("Enter proper details");
      return;
    }

    const request = {
      vendorId: this.deleteVendorModel().vendorId,
      remark: this.deleteVendorModel().remark
    };

    this.adminVendorService.DeleteVendor(request).subscribe({
      next: () => {
        this.successMessage.set("Vendor reviewed successfully");

        setTimeout(() => {
          this.closePopup();
          this.successMessage.set(null);
          this.loadVendor();
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
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  applyFilter(): void {
    this.pageNumber.set(1);
    this.loadVendor();
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
    this.loadVendor();
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
  viewVendor(vendorId : number){
     this.route.navigate(['/admin/vendors', vendorId]);
  }
}

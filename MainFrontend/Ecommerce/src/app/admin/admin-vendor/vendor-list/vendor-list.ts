import { Component, signal, computed, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { email, form, FormField, maxLength, min, pattern, required } from '@angular/forms/signals';

import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { AdminVendorFilter } from '../../../models/admin/vendor/admin-vendor.filter';
import { AdminDeleteVendorModel } from '../../../models/admin/vendor/delete-vendor.model';

import { BasePage } from '../../../shared-class/shares-page-class';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { DeletePopupComponents } from '../../../shared-components/delete-popup-components/delete-popup-components';
import { ReviewVendorModel } from '../../../models/admin/vendor/review.vendor.model';
import { ReviewPopupComponent } from '../../../shared-components/review-popup-component/review-popup-component';

@Component({
  selector: 'app-vendor-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, DataTableComponent, MobileCardComponent, PaginationComponent, FilterComponent, HeaderComponent, DeletePopupComponents, ReviewPopupComponent],
  templateUrl: './vendor-list.html',
  styleUrl: './vendor-list.css',
})

export class VendorList extends BasePage {
  constructor(private route: Router, private router: ActivatedRoute, private adminVendorService: AdminVendorService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }
  status = signal<number | null>(null);
  draftstatus = signal<number | null>(null);
  pageTitle = signal<string | null>(null);
  includeIsDeleted = signal<boolean | null>(null);
  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.includeIsDeleted.set(data['includeIsDeleted']);
      this.pageTitle.set(data['title']);
      this.loadVendor();
    });
  }
  protected loadData(): void {
    this.loadVendor();
  }

  approvalStatusOption = [
    { id: 2, label: 'Accept' },
    { id: 3, label: 'Reject' },
  ];
  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);
  totalPages = computed(() => this.vendors()?.totalPages ?? 1);
  loadVendor() {
    this.buildFilters();
    this.adminVendorService.getVendor(this.adminVendorFilter()).subscribe({
      next: (response: any) => {
        this.vendors.set(response);
      },
      error: (error) => {
        //console.log(error);
        if (error.status === 404) {
          this.vendors.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0,
          });
        } else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.errorMessage ?? '');
        }
      },
    });
  }


  adminVendorFilter = signal(new AdminVendorFilter());
  filterForm = form(this.adminVendorFilter, (path) => {
    email(path.companyEmail, { message: 'Enter a valid email address.' });
    pattern(path.companyPhoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: 'Enter a valid phone number.' });
    pattern(path.gstNumber, /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{1}Z[A-Z0-9]{1}$/, { message: 'GST Number for India is not valid.' });
    min(path.reviewedByAdminId, 1, { message: 'ID cannot be negative or 0.' });
    pattern(path.contactPersonName, /^[A-Za-z][A-Za-z\s'.-]{1,49}$/, { message: 'Enter a valid contact person name.' });
    pattern(path.vendorCompanyName, /^[A-Za-z][A-Za-z\s'.-]{1,50}$/, { message: 'Enter a valid contact person name.' });
  });

  private buildFilters() {
    this.adminVendorFilter.update(filter => ({
      ...filter,
      approvalStatusId: this.draftstatus != null && this.status() != 4 && this.status() != 1 ? this.draftstatus() : this.status(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      includeIsDeleted : this.includeIsDeleted(),
      email: filter.companyEmail.trim().toLowerCase(),
      phoneNumber: filter.companyPhoneNumber.trim().toLowerCase(),
      contactPersonName: filter.contactPersonName.trim().toLowerCase(),
      vendorCompanyName: filter.vendorCompanyName.trim().toLowerCase(),
      gstNumber: filter.gstNumber.trim().toLowerCase(),
    }));
  }

  clearFilterValues(): void {
    this.draftstatus.set(null);
    this.adminVendorFilter.set(new AdminVendorFilter());
    this.adminVendorFilter.update(filter => ({
      ...filter,
      approvalStatusId: null,
      isActive: null,
    }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.adminVendorFilter.update(filter => ({ ...filter, isActive: value === '' ? null : value === 'true' }));
  }

  onApprovalChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.draftstatus.set(v ? Number(v) : null);
  }

  showDeletePopup = signal(false);
  showReviewPopup = signal(false);

  successMessage = signal<string>('');
  errorMessage = signal<string>('');
  progress = signal(false);

  deleteVendorModel = signal(new AdminDeleteVendorModel());

  deleteForm = form(this.deleteVendorModel, (path) => {
    required(path.remark, { message: 'Enter The Remarks' });
    maxLength(path.remark, 150, { message: 'Maximum 100 characters' });
  });

  openDeletePopup(vendorId: number) {
    this.deleteVendorModel.update(model => ({
      ...model,
      vendorId,
      remark: '',
    }));
    this.showDeletePopup.set(true);
  }

  onConfirmDelete() {
    this.errorMessage.set('');
    this.successMessage.set('');

    const errors = [];
    if (this.deleteForm.remark().invalid()) {
      errors.push(this.deleteForm.remark().errors()[0].message);
    }
    this.errorMessage.set(errors.join(', '));

    if (this.deleteForm().invalid()) {
      return;
    }
    this.progress.set(true);
    const request = {
      vendorId: this.deleteVendorModel().vendorId,
      remark: this.deleteVendorModel().remark,
    };
    this.adminVendorService.DeleteVendor(request).subscribe({
      next: () => {
        this.successMessage.set('Vendor deleted successfully');
        setTimeout(() => {
          this.onCancelDelete();
          this.successMessage.set('');
          this.loadVendor();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set('');

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.error?.message ?? 'Something went wrong. Please try again.');
        }

        this.progress.set(false);
      },
    });
  }
  onCancelDelete() {
    this.showDeletePopup.set(false);
    this.deleteForm().reset();
    this.deleteVendorModel.set(new AdminDeleteVendorModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  reviewVendorModel = signal(new ReviewVendorModel());

  reviewForm = form(this.reviewVendorModel, (path) => {
    required(path.approvalStatusId, { message: "Enter The Approval Status" });
    required(path.remark, { message: "Enter The Remarks" });
    maxLength(path.remark, 150, { message: "Maximum 100 characters" });
  })

  openReviewPopup(vendorId: number) {
    this.reviewVendorModel.update(model => ({
      ...model,
      vendorId: vendorId,
      approvalStatusId: null,
      remark: ''
    }));
    this.showReviewPopup.set(true);
  }

  onConfirmReview() {
    this.errorMessage.set('');
    this.successMessage.set('');
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
        this.successMessage.set("Vendor reviewed successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.onCancelReview();
          this.successMessage.set('');
          this.loadVendor();
          this.progress.set(false);
        }, 3000);
      },
      error: (error) => {
        this.successMessage.set('');

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

  onCancelReview() {
    this.showReviewPopup.set(false);
    this.reviewForm().reset();
    this.reviewVendorModel.set(new ReviewVendorModel());
    this.errorMessage.set('');
    this.successMessage.set('');
  }
  actions: TableAction<AdminVendorModel>[] = [
    { label: 'View', color: 'green', action: 'view' },
    { label: 'Delete', color: 'red', action: 'delete', visible: vendor => vendor.approvalStatusId != 4 && this.status() == null },
    { label: 'Review', color: 'blue', action: 'review', visible: vendor => vendor.approvalStatusId == 1 && this.status() == 1 },
  ];
  columns: Column[] = [
    { key: 'vendorId', header: 'ID' },
    { key: 'vendorCompanyName', header: 'Company Name' },
    { key: 'contactPersonName', header: 'Contact Person' },
    { key: 'companyEmail', header: 'Email' },
    { key: 'companyPhoneNumber', header: 'Phone Number' },
    { key: 'approvalStatusName', header: 'Approval' },
    { key: 'isActive', header: 'Status', formatter: (value: boolean) => value ? 'Active' : 'Inactive' },
  ];

  mobileColumns = [...this.columns];

  handleAction(event: { type: string; row: AdminVendorModel }) {
    switch (event.type) {
      case 'view':
        this.viewVendor(event.row.vendorId);
        break;
      case 'delete':
        this.openDeletePopup(event.row.vendorId);
        break;
      case 'review':
        this.openReviewPopup(event.row.vendorId);
        break;
    }
  }

  viewVendor(vendorId: number) {
    this.route.navigate(['/admin/vendors', vendorId]);
  }
}
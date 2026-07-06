import { Component, signal, computed, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminVendorService } from '../../../services/admin-vendor.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminVendorModel } from '../../../models/admin/vendor/admin-vendor.model';
import { AdminVendorFilter } from '../../../models/admin/vendor/admin-vendor.filter';
import { AdminDeleteVendorModel } from '../../../models/admin/vendor/delete-vendor.model';
import { email, form, FormField, maxLength, min, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { BasePage } from '../../../shared-class/shares-page-class';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';

@Component({
  selector: 'app-vendor-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, DataTableComponent, MobileCardComponent, PaginationComponent, FilterComponent],
  templateUrl: './vendor-list.html',
  styleUrl: './vendor-list.css',
})
export class VendorList extends BasePage {

  actions: TableAction<AdminVendorModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    },
    {
      label: 'Delete',
      color: 'red',
      action: 'delete',
      visible: vendor => vendor.approvalStatusId != 4
    },

  ];
  columns: Column[] = [
    {
      key: 'vendorId',
      header: 'ID'
    },
    {
      key: 'vendorCompanyName',
      header: 'Company Name'
    },
    {
      key: 'contactPersonName',
      header: 'Contact Person'
    },
    {
      key: 'companyEmail',
      header: 'Email'
    },
    {
      key: 'companyPhoneNumber',
      header: 'Phone Number'
    },
    {
      key: 'approvalStatusName',
      header: 'Approval'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    }
  ];

  mobileColumns: Column[] = [
    {
      key: 'vendorId',
      header: 'ID'
    },
    {
      key: 'vendorCompanyName',
      header: 'Company Name'
    },
    {
      key: 'contactPersonName',
      header: 'Contact Person'
    },
    {
      key: 'companyEmail',
      header: 'Email'
    },
    {
      key: 'companyPhoneNumber',
      header: 'Phone Number'
    },
    {
      key: 'approvalStatusName',
      header: 'Approval'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    }
  ];

  handleAction(event: { type: string; row: AdminVendorModel }) {
    switch (event.type) {
      case 'view':
        this.viewVendor(event.row.vendorId);
        break;
      case 'delete':
        this.openDeletePopup(event.row.vendorId);
        break;
    }
  }


  vendors = signal<PagedResponse<AdminVendorModel> | null>(null);

  totalPages = computed(() => this.vendors()?.totalPages ?? 1);
  
  approvalStatusId = signal<number | null>(null);
  isActive = signal<boolean | null>(null);
  reviewedByAdminId = signal<number | null>(null);
  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  deleteVendorModel = signal(new AdminDeleteVendorModel());
  selectedVendorId = signal<number | null>(null);
  
  progress = signal(false);

  adminVendorFilter = signal(new AdminVendorFilter());

  filterForm = form(this.adminVendorFilter, (path) => {
    email(path.companyEmail, { message: 'Enter a valid email address.' });
    pattern(path.companyPhoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: 'Enter a valid phone number.' });
    pattern(path.gstNumber, /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{1}Z[A-Z0-9]{1}$/, { message: 'GST Number for India is not valid.' });
    min(path.reviewedByAdminId, 1);
    pattern(path.contactPersonName, /^[A-Za-z][A-Za-z\s'.-]{1,49}$/, { message: 'Enter a valid contact person name.' });
    pattern(path.vendorCompanyName, /^[A-Za-z][A-Za-z\s'.-]{1,50}$/, { message: 'Enter a valid contact person name.' });
  });

  private buildFilters() {
    this.adminVendorFilter.update(filter => ({
      ...filter,
      approvalStatusId:this.status(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      email: filter.companyEmail.trim().toLowerCase(),
      phoneNumber: filter.companyPhoneNumber.trim().toLowerCase(),
      contactPersonName: filter.contactPersonName.trim().toLowerCase(),
      vendorCompanyName: filter.vendorCompanyName.trim().toLowerCase(),
      gstNumber: filter.gstNumber.trim().toLowerCase()
    }));
  }

  constructor(private route: Router,private router:ActivatedRoute ,private adminVendorService: AdminVendorService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  protected loadData(): void {
    this.loadVendor();
  }

  status = signal<number | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadVendor();
    });
  }

  loadVendor() {
    this.buildFilters();
    this.adminVendorService.getVendor(this.adminVendorFilter()).subscribe({
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

    this.showPopup.set(true);
  }

  approvalStatusOption = [
    { id: 2, label: 'Accepted' },
    { id: 3, label: 'Rejected' },
  ]

  clearFilterValues(): void {
    this.adminVendorFilter.set(new AdminVendorFilter());
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.adminVendorFilter.update(filter => ({ ...filter, isActive: value === '' ? null : value === 'true' }));
  }

  onApprovalChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.adminVendorFilter.update(filter => ({ ...filter, approvalStatusId: v ? Number(v) : null }));
  }

  viewVendor(vendorId: number) {
    this.route.navigate(['/admin/vendors', vendorId]);
  }
}

import { Component, computed, effect, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorUserService } from '../../../services/vendor-user.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorUserModel } from '../../../models/vendor/vendor-user/response-vendor-user.model';
import { VendorUserFilter } from '../../../models/vendor/vendor-user/vendor-user.filter';
import { email, form, FormField, pattern } from '@angular/forms/signals';
import { BasePage } from '../../../shared-class/shares-page-class';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';

@Component({
  selector: 'app-vendor-user-list',
  imports: [PopupComponent, DataTableComponent, MobileCardComponent, PaginationComponent, FilterComponent, FormField, HeaderComponent],
  templateUrl: './vendor-user-list.html',
  styleUrl: './vendor-user-list.css',
})
export class VendorUserList extends BasePage {
  constructor(private router: ActivatedRoute, private route: Router, private vendorUserService: VendorUserService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  status = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadAdminUser();
    });
  }

  protected loadData(): void {
    this.loadAdminUser();
  }

  clearFilterValues(): void {
    this.adminUserFilter.set(new VendorUserFilter());
  }

  adminUsers = signal<PagedResponse<VendorUserModel> | null>(null);

  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);

  adminUserFilter = signal(new VendorUserFilter());

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);


  filterForm = form(this.adminUserFilter, (path) => {
    email(path.email, { message: 'Enter a valid email address.' });
    pattern(path.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: 'Enter a valid phone number.' });
  });

  adminRoleOption = [
    { id: 1, label: 'Owner' },
    { id: 2, label: 'Manager' },
    { id: 3, label: 'Product Manager' },
    { id: 4, label: 'Order Manager' },
    { id: 5, label: 'Return Manager' },
    { id: 6, label: 'Refund Manager' },
    { id: 7, label: 'Inventory Manager' },
    { id: 8, label: 'Coupon Manager' }
  ];

  private buildFilter() {
    this.adminUserFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      status: this.status(),
      email: filter.email.trim().toLowerCase(),
      phoneNumber: filter.phoneNumber.trim().toLowerCase()
    }));
  }

  loadAdminUser() {
    this.buildFilter();
    this.vendorUserService.getAdminUser(this.adminUserFilter()).subscribe({
      next: (response: any) => {
        this.adminUsers.set(response);
      },
      error: (error) => {
        if (error.status == 404) {
          this.adminUsers.set({
            items: [],
            totalCount: 0,
            pageNumber: 1,
            pageSize: 10,
            totalPages: 1
          });
        }
      }
    })
  }

  onAdminRoleChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.adminUserFilter.update(filter => ({ ...filter, vendorRoleId: v ? Number(v) : null }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.adminUserFilter.update(filter => ({ ...filter, status: value === '' ? null : value === 'true' }));
  }


  progress = signal(false);

  deactivateAdmin() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.progress.set(true);
    this.vendorUserService.deactivateAdminUser(id).subscribe({
      next: (response: any) => {
        this.successMessage.set('Vendor user deactivated successfully.');
        setTimeout(() => {
          this.successMessage.set('');
          this.closePopup();
          this.progress.set(false);
        }, 3000);
        this.loadAdminUser();
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
        this.errorMessage.set(
          error.error?.message ?? 'Something went wrong.'
        );
      }
    })
  }

  activateAdmin() {
    this.errorMessage.set('');
    this.successMessage.set('');
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.progress.set(true);
    this.vendorUserService.activateAdminUser(id).subscribe({
      next: (response: any) => {
        this.successMessage.set('Vendor user activated successfully.');
        setTimeout(() => {
          this.successMessage.set('');
          this.closePopup();
          this.progress.set(false);
        }, 3000);
        this.loadAdminUser();
      },
      error: (error) => {
        console.log(error);
        this.progress.set(false);
        this.errorMessage.set(
          error.error?.message ?? 'Something went wrong.'
        );
      }
    })
  }

  selectedAction = signal<'activate' | 'deactivate' | null>(null);

  confirmPopup() {
    switch (this.selectedAction()) {
      case 'activate':
        this.activateAdmin();
        break;

      case 'deactivate':
        this.deactivateAdmin();
        break;
    }
  }


  actions = computed<TableAction<VendorUserModel>[]>(() =>
    this.status() == null
      ? [
        { label: 'View', color: 'blue', action: 'view' }
      ]
      : [
        { label: 'View', color: 'blue', action: 'view' },
        { label: 'Deactivate', color: 'red', action: 'deactivate', visible: category => category.isActive && category.vendorRoleId!=1 },
        { label: 'Activate', color: 'green', action: 'activate', visible: category => !category.isActive }
      ]
  );

  columns: Column[] = [
    { key: 'vendorUserId', header: 'ID' },
    { key: 'firstName', header: 'First Name' },
    { key: 'lastName', header: 'Last Name' },
    { key: 'vendorRoleName', header: 'Role' },
    { key: 'email', header: 'Email' },
    { key: 'phoneNumber', header: 'Phone Number' },
    { key: 'isActive', header: 'Status', formatter: (value: boolean) => (value ? 'Active' : 'Inactive') }
  ];
  mobileColumns = [...this.columns];

  handleAction(event: { type: string; row: VendorUserModel }) {
    switch (event.type) {
      case 'view':
        this.viewAdminUser(event.row.vendorUserId);
        break;
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.vendorUserId);

        this.popupTitle.set('Activate Vendor User');
        this.popupMessage.set('Are you sure you want to activate this vendor user?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');
        this.loadingText.set('Activating...');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.vendorUserId);

        this.popupTitle.set('Deactivate Vendor User');
        this.popupMessage.set('Are you sure you want to deactivate this vendor user?');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');
        this.loadingText.set('Deactivating...');

        this.showPopup.set(true);
        break;
    }
  }

  viewAdminUser(productId: number) {
    this.route.navigate(['/vendor/users', productId]);
  }
}


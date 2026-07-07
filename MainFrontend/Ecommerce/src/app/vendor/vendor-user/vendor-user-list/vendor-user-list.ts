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

@Component({
  selector: 'app-vendor-user-list',
  imports: [ PopupComponent,DataTableComponent,MobileCardComponent,PaginationComponent,FilterComponent,FormField],
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

  protected loadData(): void {
    this.loadAdminUser();
  }

  clearFilterValues(): void {
    this.adminUserFilter.set(new VendorUserFilter());
  }

    actions = computed<TableAction<VendorUserModel>[]>(() => {
      if (this.status() == null) {
        return [
          {
          label: 'View',
          color: 'green',
          action: 'view'
        },
        ];
      }
  
      return [
        {
          label: 'View',
          color: 'green',
          action: 'view'
        },
        {
          label: 'Deactivate',
          color: 'red',
          action: 'deactivate',
          visible: category => category.isActive
        },
        {
          label: 'Activate',
          color: 'green',
          action: 'activate',
          visible: category => !category.isActive
        }
      ];
    });


  
  columns: Column[] = [
    {
      key: 'vendorUserId',
      header: 'ID'
    },
    {
      key: 'firstName',
      header: 'First Name'
    },
    {
      key: 'lastName',
      header: 'Last Name'
    },
    {
      key: 'vendorRoleName',
      header: 'Role'
    },
    {
      key: 'email',
      header: 'Email'
    },
    {
      key: 'phoneNumber',
      header: 'Phone Number'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    }
  ];

  mobileColumns: Column[] = [
    {
      key: 'vendorUserId',
      header: 'ID'
    },
    {
      key: 'firstName',
      header: 'First Name'
    },
    {
      key: 'lastName',
      header: 'Last Name'
    },
    {
      key: 'vendorRoleName',
      header: 'Role'
    },
    {
      key: 'email',
      header: 'Email'
    },
    {
      key: 'phoneNumber',
      header: 'Phone Number'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    }
  ];

  selectedAction = signal<'activate' | 'deactivate' | null>(null);

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

        this.showPopup.set(true);
        break;
    }
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

  adminUsers = signal<PagedResponse<VendorUserModel> | null>(null);

  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);

  adminUserFilter = signal(new VendorUserFilter());

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  filtererrorMessage = signal<string | null>(null);
  filterapplied = signal(false);

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
    this.adminUserFilter.update(filter => ({ ...filter, adminRoleId: v ? Number(v) : null }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.adminUserFilter.update(filter => ({ ...filter, status: value === '' ? null : value === 'true' }));
  }

  onPhoneNumberChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim();
    this.adminUserFilter.update(filter => ({ ...filter, phoneNumber: value }));
  }

  onEmailChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.adminUserFilter.update(filter => ({ ...filter, email: value }));
  }

  deactivateAdmin() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.vendorUserService.deactivateAdminUser(id).subscribe({
      next: (response: any) => {
        this.loadAdminUser();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  activateAdmin() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.vendorUserService.activateAdminUser(id).subscribe({
      next: (response: any) => {
        this.closePopup();
        this.loadAdminUser();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
  viewAdminUser(productId: number) {
    this.route.navigate(['/vendor/users', productId]);
  }
}


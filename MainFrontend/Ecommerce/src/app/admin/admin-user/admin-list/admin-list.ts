import { Component, signal, computed, effect } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminUserService } from '../../../services/admin-user.Service';
import { AdminUserModel } from '../../../models/admin/admin-user/admin-user.model';
import { AdminUserFilter } from '../../../models/admin/admin-user/admin-user.filter';
import { email, form, FormField, pattern } from '@angular/forms/signals';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';

@Component({
  selector: 'app-admin-list',
  imports: [FormField, PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, PopupComponent],
  templateUrl: './admin-list.html',
  styleUrl: './admin-list.css',
})
export class AdminList extends BasePage {
  constructor(private route: Router, private adminUserService: AdminUserService, private router: ActivatedRoute) {
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


  actions: TableAction<AdminUserModel>[] = [
    {
      label: 'View',
      color: 'blue',
      action: 'view'
    },
    {
      label: 'Deactivate',
      color: 'red',
      action: 'deactivate',
      visible: admin => admin.isActive
    },
    {
      label: 'Activate',
      color: 'green',
      action: 'activate',
      visible: admin => !admin.isActive
    }
  ];
  columns: Column[] = [
    {
      key: 'adminUserId',
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
      key: 'adminRoleName',
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
      key: 'adminUserId',
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
      key: 'adminRoleName',
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

  handleAction(event: { type: string; row: AdminUserModel }) {
    switch (event.type) {
      case 'view':
        this.viewAdminUser(event.row.adminUserId);
        break;
      case 'activate':
        this.selectedAction.set('activate');
        this.selectedId.set(event.row.adminUserId);

        this.popupTitle.set('Activate Admin User');
        this.popupMessage.set('Are you sure you want to activate this admin user?');
        this.popupConfirmText.set('Activate');
        this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
        this.titleClass.set('text-green-700');

        this.showPopup.set(true);
        break;

      case 'deactivate':
        this.selectedAction.set('deactivate');
        this.selectedId.set(event.row.adminUserId);

        this.popupTitle.set('Deactivate Admin User');
        this.popupMessage.set('Are you sure you want to deactivate this admin user?');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');

        this.showPopup.set(true);
        break;
    }
  }

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
        this.deleteAdminUser();
        break;
    }
  }



  status = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);
  adminUsers = signal<PagedResponse<AdminUserModel> | null>(null);
  totalPages = computed(() => this.adminUsers()?.totalPages ?? 1);

  adminUserFilter = signal(new AdminUserFilter());

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  filterForm = form(this.adminUserFilter, (path) => {
    email(path.email, { message: 'Enter a valid email address.' });
    pattern(path.phoneNumber, /^[1-9]{1}[0-9]{9}$/, { message: 'Enter a valid phone number.' });
  });

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
    this.adminUserService.getAdminUser(this.adminUserFilter()).subscribe({
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

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);
    this.adminUserFilter.update(filter => ({ ...filter, pageNumber: 1, pageSize: value }));
    this.loadAdminUser();
  }

  onAdminRoleChange(event: Event): void {
    const v = (event.target as HTMLSelectElement).value;
    this.adminUserFilter.update(filter => ({ ...filter, adminRoleId: v ? Number(v) : null }));
  }

  clearFilterValues(): void {
    this.adminUserFilter.set(new AdminUserFilter());
  }

  viewAdminUser(productId: number) {
    this.route.navigate(['/admin/users', productId]);
  }

  activateAdmin() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminUserService.activateAdminUser(id).subscribe({
      next: (response: any) => {
        this.closePopup();
        this.loadAdminUser();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }

  deleteAdminUser() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.adminUserService.deactivateAdminUser(id).subscribe({
      next: (response: any) => {
        this.loadAdminUser();
        this.closePopup();
      },
      error: (error) => {
        console.log(error);
      }
    })
  }
}

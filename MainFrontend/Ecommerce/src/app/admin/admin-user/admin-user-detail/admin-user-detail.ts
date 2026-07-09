import { Component, signal } from '@angular/core';
import { AdminUserService } from '../../../services/admin-user.Service';
import { AdminUserModel } from '../../../models/admin/admin-user/admin-user.model';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { PopupBase } from '../../../shared-class/popup-base-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { DetailedCardComponenet } from '../../../shared-components/detailed-card-componenet/detailed-card-componenet';
import { Column } from '../../../shared-components/data-table-component/column.model';

@Component({
  selector: 'app-admin-user-detail',
  imports: [PopupComponent, DetailedCardComponenet],
  providers: [DatePipe],
  templateUrl: './admin-user-detail.html',
  styleUrl: './admin-user-detail.css',
})
export class AdminUserDetail extends PopupBase {
  adminUser = signal<AdminUserModel | null>(null);
  errorMessage = signal<string | null>(null);
  showActivatePopup = signal(false);
  loading = signal(true);
  constructor(private datePipe: DatePipe, private adminUserService: AdminUserService, private route: ActivatedRoute) {
    super();
  }
  loadAdminUser(id: number) {
    this.loading.set(true);
    this.adminUserService.getAdminUserDetail(id).subscribe({
      next: (response: any) => {
        this.adminUser.set(response);
        console.log(response);
        this.loading.set(false);
      },
      error: (error) => {
        if (error.status == 404) {
          this.loading.set(false);
          this.errorMessage.set("Admin User Is Not Found");
        }
      }
    })
  }
  ngOnInit() {
    const adminUserId = Number(this.route.snapshot.paramMap.get('id'));

    if (adminUserId) {
      this.loadAdminUser(adminUserId);
    }
  }

  columns: Column[] = [
    { key: 'adminUserId', header: 'Admin ID' },
    { key: 'userId', header: 'User ID' },
    { key: 'firstName', header: 'First Name' },
    { key: 'lastName', header: 'Last Name' },
    { key: 'adminRoleName', header: 'Role' },
    { key: 'email', header: 'Email' },
    { key: 'phoneNumber', header: 'Phone Number' },
    {
      key: 'isActive',
      header: 'Status',
      formatter: value => value ? 'Active' : 'Inactive'
    },
    {
      key: 'createdAt',
      header: 'Created Date',
      formatter: value => this.datePipe.transform(value, 'dd/MM/yy') ?? ''
    }
  ];

  actions: TableAction<AdminUserModel>[] = [
    {
      label: 'Deactivate',
      color: 'red',
      action: 'deactivate',
      visible: admin => admin.isActive && admin.adminRoleId != 1
    },
    {
      label: 'Activate',
      color: 'green',
      action: 'activate',
      visible: admin => !admin.isActive
    }
  ];

  handleAction(event: { type: string; row: AdminUserModel }): void {
    const isActive = event.row.isActive;

    this.selectedId.set(event.row.adminUserId);

    if (isActive) {
      this.popupTitle.set('Deactivate Admin User');
      this.popupMessage.set('Are you sure you want to deactivate this admin user?');
      this.popupConfirmText.set('Deactivate');
      this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
      this.titleClass.set('text-red-700');
    } else {
      this.popupTitle.set('Activate Admin User');
      this.popupMessage.set('Are you sure you want to activate this admin user?');
      this.popupConfirmText.set('Activate');
      this.popupButtonClass.set('bg-green-700 hover:bg-green-900');
      this.titleClass.set('text-green-700');
    }

    this.showPopup.set(true);
  }

  confirmPopup(): void {
    const current = this.adminUser();
    if (current == null) {
      return;
    }
    this.updateAdminStatus(!current.isActive);
  }

  updateAdminStatus(isActivate: boolean): void {
    const id = this.adminUser()?.adminUserId;
    if (id == null) {
      return;
    }
    const request = isActivate ? this.adminUserService.activateAdminUser(id) : this.adminUserService.deactivateAdminUser(id);
    request.subscribe({
      next: () => {
        this.closePopup();
        this.loadAdminUser(id);
      },
      error: error => console.log(error)
    });
  }
}

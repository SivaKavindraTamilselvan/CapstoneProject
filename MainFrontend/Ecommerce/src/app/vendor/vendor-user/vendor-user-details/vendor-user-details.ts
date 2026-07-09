import { Component, signal } from '@angular/core';
import { VendorUserModel } from '../../../models/vendor/vendor-user/response-vendor-user.model';
import { VendorUserService } from '../../../services/vendor-user.Service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { DetailedCardComponenet } from '../../../shared-components/detailed-card-componenet/detailed-card-componenet';
import { PopupBase } from '../../../shared-class/popup-base-class';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';

@Component({
  selector: 'app-vendor-user-details',
  imports: [DetailedCardComponenet, PopupComponent],
  providers: [DatePipe],
  templateUrl: './vendor-user-details.html',
  styleUrl: './vendor-user-details.css',
})
export class VendorUserDetails extends PopupBase {
  adminUser = signal<VendorUserModel | null>(null);
  errorMessage = signal<string | null>(null);
  showActivatePopup = signal(false);
    loading = signal(true);
  constructor(private datePipe: DatePipe, private vendorUserService: VendorUserService, private route: ActivatedRoute) {
    super();
  }
  loadAdminUser(id: number) {
    this.vendorUserService.getAdminUserDetail(id).subscribe({
      next: (response: any) => {
        this.adminUser.set(response);
        console.log(response);
        this.loading.set(false);
      },
      error: (error) => {
        if (error.status == 404) {
          this.errorMessage.set("Admin User Is Not Found");
          this.loading.set(false);
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
    { key: 'vendorUserId', header: 'Vendor User ID' },
    { key: 'userId', header: 'User ID' },
    { key: 'firstName', header: 'First Name' },
    { key: 'lastName', header: 'Last Name' },
    { key: 'vendorRoleName', header: 'Role' },
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

  actions: TableAction<VendorUserModel>[] = [
    {
      label: 'Deactivate',
      color: 'red',
      action: 'deactivate',
      visible: admin => admin.isActive && admin.vendorRoleId!=1
    },
    {
      label: 'Activate',
      color: 'green',
      action: 'activate',
      visible: admin => !admin.isActive 
    }
  ];

  handleAction(event: { type: string; row: VendorUserModel }): void {
    const isActive = event.row.isActive;

    this.selectedId.set(event.row.vendorUserId);

    if (isActive) {
      this.popupTitle.set('Deactivate Vendor User');
      this.popupMessage.set('Are you sure you want to deactivate this vendor user?');
      this.popupConfirmText.set('Deactivate');
      this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
      this.titleClass.set('text-red-700');
    } else {
      this.popupTitle.set('Activate Vendor User');
      this.popupMessage.set('Are you sure you want to activate this vendor user?');
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
    const id = this.adminUser()?.vendorUserId;
    if (id == null) {
      return;
    }
    const request = isActivate ? this.vendorUserService.activateAdminUser(id) : this.vendorUserService.deactivateAdminUser(id);
    request.subscribe({
      next: () => {
        this.closePopup();
        this.loadAdminUser(id);
      },
      error: error => console.log(error)
    });
  }

}


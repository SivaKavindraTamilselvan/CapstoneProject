import { Component, signal } from '@angular/core';
import { AddressModel } from '../../../models/address/address-response.model';
import { ActivatedRoute, Router } from '@angular/router';
import { AddressService } from '../../../services/address.Service';
import { DatePipe } from '@angular/common';
import { PopupBase } from '../../../shared-class/popup-base-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { DetailedCardComponenet } from '../../../shared-components/detailed-card-componenet/detailed-card-componenet';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { AddInventoryComponent } from '../add-inventory-component/add-inventory-component';

@Component({
  selector: 'app-get-warehouse-address',
  imports: [PopupComponent, DetailedCardComponenet, AddInventoryComponent],
  providers: [DatePipe],
  templateUrl: './get-warehouse-address.html',
  styleUrl: './get-warehouse-address.css',
})
export class GetWarehouseAddress extends PopupBase {
  address = signal<AddressModel | null>(null);
  errorMessage = signal<string | null>(null);
  loading = signal(true);

  constructor(
    private datePipe: DatePipe,
    private addressService: AddressService,
    private inventoryService: VendorInventoryService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    super();
  }

  ngOnInit(): void {
    const addressId = Number(this.route.snapshot.paramMap.get('id'));

    if (addressId) {
      this.loadAddress(addressId);
    }
  }

  loadAddress(id: number) {
    this.loading.set(true);
    this.addressService.getAddressId(id).subscribe({
      next: (response: any) => {
        this.address.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        this.loading.set(false);
        if (error.status === 0) {
          this.errorMessage.set(
            'Unable to connect to the server. Please check your internet connection.'
          );
        }
        else if (error.status === 404) {
          this.errorMessage.set('Address not found.');
        }
        else if (error.status >= 500) {
          this.errorMessage.set(
            'Something went wrong on the server. Please try again later.'
          );
        }
        else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        }
        else {
          this.errorMessage.set('Failed to load address.');
        }
      }
    });
  }

  columns: Column[] = [
    { key: 'addressId', header: 'Address ID' },
    { key: 'contactName', header: 'Contact Name' },
    { key: 'contactPhoneNumber', header: 'Phone Number' },
    { key: 'addressLine', header: 'Address Line' },
    { key: 'landMark', header: 'Landmark' },
    { key: 'city', header: 'City' },
    { key: 'state', header: 'State' },
    { key: 'pinCode', header: 'Pin Code' },
    {
      key: 'isDefault',
      header: 'Default',
      formatter: value => value ? 'Yes' : 'No'
    },
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

  actions: TableAction<AddressModel>[] = [
    { label: 'Add Inventory', color: 'green', action: 'add', visible: address => address.isActive },
    { label: 'Delete', color: 'red', action: 'delete', visible: address => address.isActive }
  ];

  handleAction(event: { type: string; row: AddressModel }) {
    switch (event.type) {

      case 'delete':
        this.selectedId.set(event.row.addressId);
        this.popupTitle.set('Delete Warehouse');
        this.popupMessage.set('Are you sure you want to delete the warehouse? If once deleted cannot be recovered.');
        this.popupConfirmText.set('Delete Warehouse');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');

        this.showPopup.set(true);
        break;
      case 'add':
        this.openInventoryPopup(event.row.addressId);
        break;
      
    }
  }
  successMessage = signal('');
  progress = signal(false);
  selectedAddressId = signal<number | null>(null);

  confirmPopup() {
    this.deleteAddress();
  }

  confirmDeactivate(id: number) {
    this.selectedAddressId.set(id);
    this.showPopup.set(true);
  }

  deleteAddress() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }

    this.progress.set(true);
    this.errorMessage.set(null);
    this.successMessage.set('');

    this.addressService.deleteAddress(id).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Warehouse deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.successMessage.set('');
          this.closePopup();
          this.loadAddress(id);
        }, 3000);
      },
      error: (error) => {
        this.progress.set(false);
        this.successMessage.set('');

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    });
  }

  showAddInventoryPopup = signal(false);
  selectedAddressIdForInventory = signal<number | null>(null);

  openInventoryPopup(addressId: number) {
    this.selectedAddressIdForInventory.set(addressId);
    this.showAddInventoryPopup.set(true);
  }

  closeInventoryPopup() {
    this.showAddInventoryPopup.set(false);
    this.selectedAddressIdForInventory.set(null);
  }

  goBack(): void {
    this.router.navigate(['/vendor/warehouses']);
  }
}
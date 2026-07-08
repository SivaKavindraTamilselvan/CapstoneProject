import { Component, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddressModel } from '../../../models/address/address-response.model';
import { INDIA_STATES } from '../../../constant/indian-state.constant';
import { AddressService } from '../../../services/address.Service';
import { AddressFilter } from '../../../models/address/address-filter';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { VendorWarehouseService } from '../../../services/vendor-warehouse.Service';
import { BasePage } from '../../../shared-class/shares-page-class';
import { ActivatedRoute } from '@angular/router';
import { form, FormField, maxLength, min, pattern, required } from '@angular/forms/signals';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddInventoryModel } from '../../../models/inventory/add-inventory.model';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';

@Component({
  selector: 'app-vendor-warehouse-list',
  imports: [PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, PopupComponent, FormsModule, FormField, ReactiveFormsModule],
  templateUrl: './vendor-warehouse-list.html',
  styleUrl: './vendor-warehouse-list.css',
})
export class VendorWarehouseList extends BasePage {

  actions = computed<TableAction<AddressModel>[]>(() => {
    if (this.pageTitle() === 'Add Inventory') {
      return [
        {
          label: 'Add Inventory',
          color: 'green',
          action: 'add',
        }
      ];
    }

    return [
      {
        label: 'View',
        color: 'green',
        action: 'view',
      },
      {
        label: 'Delete',
        color: 'red',
        action: 'delete',
        visible: address => address.isActive
      }
    ];
  });

  columns: Column[] = [
    {
      key: 'addressId',
      header: 'ID'
    },
    {
      key: 'contactName',
      header: 'Contact Name'
    },
    {
      key: 'contactPhoneNumber',
      header: 'Contact Phone'
    },
    {
      key: 'city',
      header: 'City'
    },
    {
      key: 'state',
      header: 'State',
    },
    {
      key: 'pinCode',
      header: 'PinCode'
    },

  ];

  mobileColumns: Column[] = [
    {
      key: 'contactName',
      header: 'Contact Name'
    },
    {
      key: 'contactPhoneNumber',
      header: 'Contact Phone'
    },
    {
      key: 'city',
      header: 'City'
    },
    {
      key: 'state',
      header: 'State',
    },
    {
      key: 'pinCode',
      header: 'PinCode'
    },
  ];

  handleAction(event: { type: string; row: AddressModel }) {
    switch (event.type) {

      case 'delete':
        this.selectedAction.set('delete');
        this.selectedId.set(event.row.addressId);

        this.popupTitle.set('Delete Warehouse');
        this.popupMessage.set('Are you sure you want to delete the warehouse? If once deleted cannot be recovered.');
        this.popupConfirmText.set('Deactivate');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');

        this.showPopup.set(true);
        break;
      case 'add':
        this.openInventoryPopup(event.row.addressId);
        break;
    }
  }
  address = signal<PagedResponse<AddressModel> | null>(null);

  contactPhoneNumber = signal<string>('');
  city = signal<string>('');
  state = signal<string[]>(INDIA_STATES);
  selectedState = signal<string>('');
  pincode = signal<string>('');
  status = signal<boolean | null>(null);

  totalPages = computed(() => this.address()?.totalPages ?? 1);
  selectedAddressId = signal<number | null>(null);
  showDeactivatePopup = signal(false);

  errorMessage = signal<string | null>(null);

  filterapplied = signal(false);
  filtererrorMessage = signal<string | null>(null);

  addressFilter = signal(new AddressFilter());

  clearFilterValues(): void {
    this.selectedState.set('');
    this.addressFilter.set(new AddressFilter());
  }


  constructor(private router: ActivatedRoute, private addressService: VendorWarehouseService, private vendorInventoryService: VendorInventoryService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });

  }
  addressStatus = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.addressStatus.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadAddress();
    });
  }

  selectedAction = signal<'activate' | 'delete' | null>(null);


  confirmPopup() {
    switch (this.selectedAction()) {

      case 'delete':
        this.deleteAddress();
        break;
    }
  }

  protected loadData(): void {
    this.loadAddress();
  }

  filterForm = form(this.addressFilter, (path) => {
    pattern(path.contactPhoneNumber, /^[6-9]\d{9}$/, {
      message: 'Enter a valid 10-digit Indian phone number',
    });

    pattern(path.city, /^[A-Za-z][A-Za-z\s-]*$/, {
      message: 'City can contain only letters, spaces, and hyphens.',
    });

    maxLength(path.city, 100, {
      message: 'City cannot exceed 100 characters.',
    });

    pattern(path.state, /^[A-Za-z][A-Za-z\s-]*$/, {
      message: 'State can contain only letters, spaces, and hyphens.',
    });

    maxLength(path.state, 100, {
      message: 'State cannot exceed 100 characters.',
    });

    pattern(path.pinCode, /^[1-9][0-9]{5}$/, {
      message: 'Enter a valid 6-digit pin code.',
    });

    min(path.pageNumber, 1, {
      message: 'Page number must be at least 1.',
    });

    min(path.pageSize, 1, {
      message: 'Page size must be at least 1.',
    });
  });

  loadAddress() {
    this.buildFilter();
    this.addressService.getWarehouseAddress(this.addressFilter()).subscribe({
      next: (response: any) => {
        this.address.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);
        if (error.status == 404) {
          this.address.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1
          });
        }
        else if (error.status === 0) {
          this.errorMessage.set(
            'Unable to connect to the server. Please check your internet connection.'
          );
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
          this.errorMessage.set('Failed to load warehouses.');
        }
      }
    })
  }

  private buildFilter() {
    this.addressFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: this.addressStatus(),
      city: this.addressFilter().city.trim().toLocaleLowerCase(),
      state: this.addressFilter().state.trim().toLocaleLowerCase(),
      pinCode: this.addressFilter().pinCode.trim().toLocaleLowerCase(),
      contactPhoneNumber: this.addressFilter().contactPhoneNumber.trim().toLocaleLowerCase(),
    }));
  }

  onStateChange(event: Event) {
    const state = ((event.target as HTMLSelectElement).value);
    this.selectedState.set(state);
    this.addressFilter.update(model => ({
      ...model,
      state: state,
      remark: ''
    }));
  }

  onPinCodeChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.pincode.set(value);
  }

  onCityChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.city.set(value);
  }

  onPhoneNumberChange(event: Event) {
    this.contactPhoneNumber.set((event.target as HTMLInputElement).value);
  }



  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadAddress();
  }

  confirmDeactivate(id: number) {
    this.selectedAddressId.set(id);
    this.showDeactivatePopup.set(true);
  }

  deleteAddress() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }
    this.addressService.deleteWarehouseAddress(id).subscribe({
      next: (response: any) => {
        alert("Warehouse deleted");
        this.loadAddress();
      }
    })
  }

  showAddInventoryPopup = signal(false);
  inventorySuccess = signal<string | null>(null);
  inventoryError = signal<string | null>(null);


  inventoryModel = signal(new AddInventoryModel());

  addForm = form(this.inventoryModel, (path) => {
    required(path.addressId, { message: 'Choose The Address' });
    required(path.availableQuantity, { message: 'Enter the available quantity' });
    required(path.reservedQuantity, { message: 'Enter the reserved quantity' });
    required(path.productVariantId, { message: 'Choode the productVariant' });

  })
  openInventoryPopup(addressId: number) {
    this.selectedAddressId.set(addressId);
    this.inventoryModel.update((i) => ({ ...i, addressId: addressId }));
    this.showAddInventoryPopup.set(true);
  }
  closeInventoryPopup() {
    this.showAddInventoryPopup.set(false);
    this.inventoryModel.set(new AddInventoryModel());
    this.selectedAddressId.set(null);
    this.inventoryError.set(null);
  }
  handleAddInventory() {
    this.inventoryError.set(null);
    this.inventorySuccess.set(null);
    if (this.addForm().invalid()) {
      this.inventoryError.set("Enter proper details");
    }
    this.vendorInventoryService.addInventory(this.inventoryModel()).subscribe({
      next: (response: any) => {
        this.inventorySuccess.set("Inventory added successfully");
        setTimeout(() => {
          this.closePopup();
          this.inventorySuccess.set(null);
          this.loadAddress();
        }, 3000);
      },
      error: (error) => {
        this.inventorySuccess.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.inventoryError.set(messages);
        }
        else {
          this.inventoryError.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    })
  }
}

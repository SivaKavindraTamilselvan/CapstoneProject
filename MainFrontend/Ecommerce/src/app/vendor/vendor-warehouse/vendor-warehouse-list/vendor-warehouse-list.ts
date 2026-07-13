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
import { ActivatedRoute, Router } from '@angular/router';
import { form, FormField, maxLength, min, pattern, required } from '@angular/forms/signals';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddInventoryModel } from '../../../models/inventory/add-inventory.model';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { AddInventoryComponent } from '../add-inventory-component/add-inventory-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';

@Component({
  selector: 'app-vendor-warehouse-list',
  imports: [PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent, PopupComponent, FormsModule, FormField, ReactiveFormsModule, AddInventoryComponent,HeaderComponent],
  templateUrl: './vendor-warehouse-list.html',
  styleUrl: './vendor-warehouse-list.css',
})
export class VendorWarehouseList extends BasePage {

  actions = computed<TableAction<AddressModel>[]>(() =>
    this.pageTitle() === 'Add Inventory'
      ? [
        { label: 'Add Inventory', color: 'green', action: 'add' }
      ]
      : [
        { label: 'View', color: 'green', action: 'view' },
        { label: 'Delete', color: 'red', action: 'delete', visible: address => address.isActive }
      ]
  );

  columns: Column[] = [
    { key: 'addressId', header: 'ID' },
    { key: 'contactName', header: 'Contact Name' },
    { key: 'contactPhoneNumber', header: 'Contact Phone' },
    { key: 'city', header: 'City' },
    { key: 'state', header: 'State' },
    { key: 'pinCode', header: 'PinCode' }
  ];

  mobileColumns = [...this.columns];

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
      case 'view':
        this.viewWarehouse(event.row.addressId);
        break;
    }
  }
  address = signal<PagedResponse<AddressModel> | null>(null);


  state = signal<string[]>(INDIA_STATES);
  selectedState = signal<string>('');
  status = signal<boolean | null>(null);

  totalPages = computed(() => this.address()?.totalPages ?? 1);
  selectedAddressId = signal<number | null>(null);

  errorMessage = signal<string | null>(null);

  addressFilter = signal(new AddressFilter());

  clearFilterValues(): void {
    this.selectedState.set('');
    this.addressFilter.set(new AddressFilter());
  }

  constructor(private route : Router,private router: ActivatedRoute, private addressService: VendorWarehouseService, private vendorInventoryService: VendorInventoryService) {
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

  confirmPopup() {
    this.deleteAddress();
  }

  protected loadData(): void {
    this.loadAddress();
  }

  filterForm = form(this.addressFilter, (path) => {
    pattern(path.contactPhoneNumber, /^[6-9]\d{9}$/, { message: 'Enter a valid 10-digit Indian phone number', });
    pattern(path.city, /^[A-Za-z][A-Za-z\s-]*$/, { message: 'City can contain only letters, spaces, and hyphens.', });
    maxLength(path.city, 100, { message: 'City cannot exceed 100 characters.', });
    pattern(path.state, /^[A-Za-z][A-Za-z\s-]*$/, { message: 'State can contain only letters, spaces, and hyphens.', });
    maxLength(path.state, 100, { message: 'State cannot exceed 100 characters.', });
    pattern(path.pinCode, /^[1-9][0-9]{5}$/, { message: 'Enter a valid 6-digit pin code.', });
    min(path.pageNumber, 1, { message: 'Page number must be at least 1.', });
    min(path.pageSize, 1, { message: 'Page size must be at least 1.', });
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

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadAddress();
  }

  confirmDeactivate(id: number) {
    this.selectedAddressId.set(id);
    this.showPopup.set(true);
  }

  successMessage = signal<string | null>(null);
  progress = signal(false);

  deleteAddress() {
    const id = this.selectedId();
    if (id == null) {
      return;
    }

    this.progress.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.addressService.deleteWarehouseAddress(id).subscribe({
      next: () => {
        this.progress.set(false);
        this.successMessage.set("Warehouse deleted successfully. Closing in 3 seconds...");
        setTimeout(() => {
          this.successMessage.set(null);
          this.closePopup();
          this.loadAddress();
        }, 3000);
      },
      error: (error) => {
        this.progress.set(false);
        this.successMessage.set(null);

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
  viewWarehouse(id :number){
    this.route.navigate(['vendor/warehouses',id]);
  }
}

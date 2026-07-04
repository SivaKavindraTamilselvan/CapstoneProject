import { Component, computed, signal } from '@angular/core';
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

@Component({
  selector: 'app-vendor-warehouse-list',
  imports: [PaginationComponent, FilterComponent, DataTableComponent, MobileCardComponent],
  templateUrl: './vendor-warehouse-list.html',
  styleUrl: './vendor-warehouse-list.css',
})
export class VendorWarehouseList {
  actions: TableAction[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    },
    {
      label: 'Delete',
      color: 'red',
      action: 'delete'
    }
  ];
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
        this.confirmDeactivate(event.row.addressId);
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
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.address()?.totalPages ?? 1);
  selectedAddressId = signal<number | null>(null);
  showDeactivatePopup = signal(false);

  errorMessage = signal<string | null>(null);

  filterPanelOpen = signal<boolean>(false);
  filterapplied = signal(false);
  filtererrorMessage = signal<string | null>(null);

  constructor(private addressService: VendorWarehouseService) {

  }
  ngOnInit() {
    this.loadAddress();
  }
  loadAddress() {
    this.addressService.getWarehouseAddress(this.buildFilter()).subscribe({
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
        if (error.status === 0) {
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
  private buildFilter(): AddressFilter {
    this.status.set(true);
    return {
      contactPhoneNumber: this.contactPhoneNumber(),
      city: this.city(),
      state: this.selectedState(),
      pinCode: this.pincode(),
      isActive: this.status(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  toggleFilterPanel(): void {
    const wasOpen = this.filterPanelOpen();
    this.filterPanelOpen.update((open) => !open);
    if (wasOpen && !this.filterapplied()) {
      this.resetFilters();
    }
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  applyFilters(): void {
    if (this.filtererrorMessage()) {
      return;
    }
    this.filterapplied.set(true);
    this.pageNumber.set(1);
    this.loadAddress();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.filtererrorMessage.set("");
    this.filterapplied.set(false);
    this.city.set('');
    this.selectedState.set('');
    this.pincode.set('');
    this.contactPhoneNumber.set('');
    this.pageNumber.set(1);
    this.loadAddress();
  }

  onStateChange(event: Event) {
    this.selectedState.set((event.target as HTMLSelectElement).value);
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

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.loadAddress();
  }
  nextPage(): void {
    this.goToPage(this.pageNumber() + 1);
  }

  previousPage(): void {
    this.goToPage(this.pageNumber() - 1);
  }

  onPageSizeChanged(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadAddress();
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
  closePopup() {
    this.showDeactivatePopup.set(false);
    this.selectedAddressId.set(null);
  }

  deleteAddress() {
    const id = this.selectedAddressId();
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
}

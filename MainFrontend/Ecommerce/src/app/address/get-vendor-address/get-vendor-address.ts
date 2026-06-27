import { Component, computed, signal } from '@angular/core';
import { AddressService } from '../../services/address.Service';
import { PagedResponse } from '../../models/paged-response.model';
import { AddressModel } from '../../models/address/address-response.model';
import { AddressFilter } from '../../models/address/address-filter';
import { INDIA_STATES } from '../../constant/indian-state.constant';

@Component({
  selector: 'app-get-vendor-address',
  imports: [],
  templateUrl: './get-vendor-address.html',
  styleUrl: './get-vendor-address.css',
})
export class GetVendorAddress {
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

  filterPanelOpen = signal<boolean | null>(false);

  constructor(private addressService: AddressService) {

  }
  ngOnInit() {
    this.loadAddress();
  }
  loadAddress() {
    this.addressService.getAddress(this.buildFilter()).subscribe({
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
      }
    })
  }
  private buildFilter(): AddressFilter {
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
  toggleFilterPanel() {
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel() {
    this.filterPanelOpen.set(false);
  }

  onStateChange(event: Event) {
    this.selectedState.set((event.target as HTMLSelectElement).value);
  }

  onPinCodeChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.pincode.set(value);
  }

  onPhoneNumberChange(event: Event) {
    this.contactPhoneNumber.set((event.target as HTMLInputElement).value);
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadAddress();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.city.set('');
    this.selectedState.set('');
    this.pincode.set('');
    this.contactPhoneNumber.set('');
    this.status.set(null);
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

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadAddress();
  }
}

import { Component, computed, signal } from '@angular/core';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { AddressService } from '../../../services/address.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { AddressModel } from '../../../models/address/address-response.model';
import { INDIA_STATES } from '../../../indian-state.constant';
import { AddressFilter } from '../../../models/address/address-filter';
import { form, FormField, required } from '@angular/forms/signals';
import { AddInventoryModel } from '../../../models/inventory/add-inventory.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { VendorProductService } from '../../../services/vendor-product.Service';

@Component({
  selector: 'app-add-inventory',
  imports: [FormField,ReactiveFormsModule,FormsModule],
  templateUrl: './add-inventory.html',
  styleUrl: './add-inventory.css',
})
export class AddInventory {
  constructor(private vendorInventoryService: VendorInventoryService, private addressService: AddressService,private vendorProductService : VendorProductService) {

  }
  address = signal<PagedResponse<AddressModel> | null>(null);
  inventoryModel = signal(new AddInventoryModel());

  contactPhoneNumber = signal<string>('');
  city = signal<string>('');
  state = signal<string[]>(INDIA_STATES);
  selectedState = signal<string>('');
  pincode = signal<string>('');
  status = signal<boolean>(true);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.address()?.totalPages ?? 1);

  filterPanelOpen = signal<boolean | null>(false);

  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  showPopUp = signal(false);
  selectedAddressId = signal<number | null>(null);

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

  addForm = form(this.inventoryModel, (path) => {
    required(path.addressId, { message: 'Choose The Address' });
    required(path.availableQuantity, { message: 'Enter the available quantity' });
    required(path.reservedQuantity, { message: 'Enter the reserved quantity' });
    required(path.productVariantId, { message: 'Choode the productVariant' });

  })
  openAddPopup(addressId: number) {
    this.selectedAddressId.set(addressId);
    this.inventoryModel.update((i) => ({ ...i, addressId: addressId }));
    this.showPopUp.set(true);
  }
  closePopup() {
    this.showPopUp.set(false);
    this.inventoryModel.set(new AddInventoryModel());
    this.selectedAddressId.set(null);
    this.errorMessage.set(null);
  }
  handleAddInventory() {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    if (this.addForm().invalid()) {
      this.errorMessage.set("Enter proper details");
    }
    this.vendorInventoryService.addInventory(this.inventoryModel()).subscribe({
      next: (response: any) => {
        this.successMessage.set("Inventory added successfully");
        setTimeout(()=>{
          this.closePopup();
          this.successMessage.set(null);
          this.loadAddress();
        },3000);
      },
      error: (error) => {
        this.successMessage.set(null);

        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(", ");

          this.errorMessage.set(messages);
        }
        else {
          this.errorMessage.set(
            error.error?.message ?? "Something went wrong. Please try again."
          );
        }
      }
    })
  }
}

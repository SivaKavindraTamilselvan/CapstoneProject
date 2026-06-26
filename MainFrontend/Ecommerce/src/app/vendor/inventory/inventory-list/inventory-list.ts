import { Component, computed, signal } from '@angular/core';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorInventoryModel } from '../../../models/inventory/inventory.model';
import { VendorInventoryFilterModel } from '../../../models/inventory/inventory.filter';

@Component({
  selector: 'app-inventory-list',
  imports: [],
  templateUrl: './inventory-list.html',
  styleUrl: './inventory-list.css',
})
export class InventoryList {
  inventoryList = signal<PagedResponse<VendorInventoryModel> | null>(null);

  addressId = signal<number | null>(null);
  productVariantId = signal<number | null>(null);
  minimumAvailableQuantity = signal<number | null>(null);
  minimumReservedQuantity = signal<number | null>(null);
  maximumAvailableQuantity = signal<number | null>(null);
  maximumReservedQuantity = signal<number | null>(null);
  status = signal<boolean | null>(null);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = computed(() => this.inventoryList()?.totalPages ?? 1);
  filterPanelOpen = signal<boolean>(false);

  constructor(private inventoryService: VendorInventoryService) {

  }
  ngOnInit(){
    this.loadInventory();
  }
  loadInventory() {
    this.inventoryService.getInventory(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.inventoryList.set(response);
        console.log(this.inventoryList());
      },
      error: (error) => {
        console.log(error);
        if (error.status == 404) {
          this.inventoryList.set({
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
  private buildFilter(): VendorInventoryFilterModel {
    return {
      addressId: this.addressId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      productVariantId: this.productVariantId() || null,
      minimumAvailableQuantity: this.minimumAvailableQuantity(),
      minimumReservedQuantity: this.minimumReservedQuantity(),
      maximumAvailableQuantity: this.maximumAvailableQuantity(),
      maximumReservedQuantity: this.maximumReservedQuantity(),
      status: this.status(),
    };
  }
  toggleFilterPanel(): void {
    this.filterPanelOpen.update((open) => !open);
  }
  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }
  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadInventory();
    this.closeFilterPanel();
  }
  resetFilters(): void {
    this.pageNumber.set(1);
    this.addressId.set(null);
    this.productVariantId.set(null);
    this.status.set(null);
    this.maximumAvailableQuantity.set(null);
    this.minimumAvailableQuantity.set(null);
    this.maximumReservedQuantity.set(null);
    this.minimumReservedQuantity.set(null);
    this.loadInventory();
    this.closeFilterPanel();
  }
  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages()) {
      return;
    }
    this.pageNumber.set(pageNumber);
    this.loadInventory();
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
    this.loadInventory();
  }

  onAddressInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.addressId.set(v ? Number(v) : null);
  }

  onProductVariantInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.productVariantId.set(v ? Number(v) : null);
  }

  onMinAvailableInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minimumAvailableQuantity.set(v ? Number(v) : null);
  }

  onMaxAvailableInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maximumAvailableQuantity.set(v ? Number(v) : null);
  }

  onMinReservedInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minimumReservedQuantity.set(v ? Number(v) : null);
  }

  onMaxReservedInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maximumReservedQuantity.set(v ? Number(v) : null);
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (value === '') {
      this.status.set(null);
    }
    else {
      this.status.set(value === 'true');
    }
  }
}

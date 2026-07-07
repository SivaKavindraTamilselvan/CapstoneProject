import { Component, computed, effect, signal } from '@angular/core';
import { VendorInventoryService } from '../../../services/vendor-inventory.Service';
import { PagedResponse } from '../../../models/paged-response.model';
import { VendorInventoryModel } from '../../../models/inventory/inventory.model';
import { VendorInventoryFilterModel } from '../../../models/inventory/inventory.filter';
import { ActivatedRoute, Router } from '@angular/router';
import { UpdateInventoryModel } from '../../../models/inventory/update-inventory.model';
import { form, FormField, min, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { PopupComponent } from '../../../shared-components/popup-component/popup-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';

@Component({
  selector: 'app-inventory-list',
  imports: [FormField, ReactiveFormsModule, FormsModule, PopupComponent, MobileCardComponent, DataTableComponent, FilterComponent, PaginationComponent],
  templateUrl: './inventory-list.html',
  styleUrl: './inventory-list.css',
})
export class InventoryList extends BasePage {

  actions: TableAction<VendorInventoryModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view',
    },
    {
      label: 'Update',
      color: 'blue',
      action: 'update',
      visible: address => address.isActive
    },


    {
      label: 'Delete',
      color: 'red',
      action: 'delete',
      visible: address => address.isActive
    }
  ];
  columns: Column[] = [
    {
      key: 'inventoryId',
      header: 'ID'
    },
    {
      key: 'addressId',
      header: 'Contact Name'
    },
    {
      key: 'sku',
      header: 'Contact Phone'
    },
    {
      key: 'availableQuantity',
      header: 'City'
    },
    {
      key: 'reservedQuantity',
      header: 'State',
    },
    {
      key: 'pinCode',
      header: 'PinCode'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },

  ];

  mobileColumns: Column[] = [
    {
      key: 'addressId',
      header: 'Contact Name'
    },
    {
      key: 'sku',
      header: 'Contact Phone'
    },
    {
      key: 'availableQuantity',
      header: 'City'
    },
    {
      key: 'reservedQuantity',
      header: 'State',
    },
    {
      key: 'pinCode',
      header: 'PinCode'
    },
    {
      key: 'isActive',
      header: 'Status',
      formatter: (value: boolean) => value ? 'Active' : 'Inactive'
    },
  ];

  handleAction(event: { type: string; row: VendorInventoryModel }) {
    switch (event.type) {

      case 'delete':
        this.selectedAction.set('delete');
        this.selectedId.set(event.row.inventoryId);

        this.popupTitle.set('Delete Inventory');
        this.popupMessage.set('Are you sure you want to delete the inventory? If once deleted cannot be recovered.');
        this.popupConfirmText.set('Delete');
        this.popupButtonClass.set('bg-red-700 hover:bg-red-900');
        this.titleClass.set('text-red-700');

        this.showPopup.set(true);
        break;
      case 'view':
        this.viewInventory(event.row.inventoryId);
        break;
      case 'update':
        this.confirmUpdate(event.row.inventoryId);
        break;
    }
  }

  inventoryFilter = signal(new VendorInventoryFilterModel());


  clearFilterValues(): void {
    this.inventoryFilter.set(new VendorInventoryFilterModel());
  }
  inventoryList = signal<PagedResponse<VendorInventoryModel> | null>(null);

  addressId = signal<number | null>(null);
  productVariantId = signal<number | null>(null);
  minimumAvailableQuantity = signal<number | null>(null);
  minimumReservedQuantity = signal<number | null>(null);
  maximumAvailableQuantity = signal<number | null>(null);
  maximumReservedQuantity = signal<number | null>(null);
  status = signal(true);

  totalPages = computed(() => this.inventoryList()?.totalPages ?? 1);

  selectedInvetoryId = signal<number | null>(null);
  showDeactivatePopup = signal(false);

  showUpdatePopup = signal(false);
  updateModel = signal(new UpdateInventoryModel());


  constructor(private inventoryService: VendorInventoryService, private route: Router, private router: ActivatedRoute) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }
  inevntoryStatus = signal<boolean | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.inevntoryStatus.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadInventory();
    });
  }

  selectedAction = signal<'activate' | 'delete' | null>(null);


  confirmPopup() {
    switch (this.selectedAction()) {

      case 'delete':
        this.deleteInventory();
        break;
    }
  }

  protected loadData(): void {
    this.loadInventory();
  }
  filterForm = form(this.inventoryFilter, (path) => {
    min(path.productVariantId, 1, { message: 'Product Variant ID must be greater than 0.' });
    min(path.addressId, 1, { message: 'Address ID must be greater than 0.' });
    min(path.minimumAvailableQuantity, 0, { message: 'Minimum available quantity cannot be negative.' });
    min(path.maximumAvailableQuantity, 0, { message: 'Maximum available quantity cannot be negative.' });
    min(path.minimumReservedQuantity, 0, { message: 'Minimum reserved quantity cannot be negative.' });
    min(path.maximumReservedQuantity, 0, { message: 'Maximum reserved quantity cannot be negative.' });
    min(path.pageNumber, 1, { message: 'Page number must be at least 1.' });
    min(path.pageSize, 1, { message: 'Page size must be at least 1.' });
  });

  loadInventory() {
    this.buildFilter();
    this.inventoryService.getInventory(this.inventoryFilter()).subscribe({
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

  private buildFilter() {
    this.inventoryFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: this.inevntoryStatus(),

    }));
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

  viewInventory(inventoryId: number) {
    this.route.navigate(['/vendor/inventory-details', inventoryId]);
  }


  updateForm = form(this.updateModel, (path) => {
    required(path.availableQuantity, { message: 'Enter The currently available Quantity' });
    required(path.inventoryId, { message: 'Choose The Inventory Id' });
  });

  updateInventory() {
    const inventoryId = this.selectedInvetoryId();
    if (inventoryId == null) {
      return;
    }
    this.updateModel.update((i) => ({ ...i, inventoryId: inventoryId }));
    this.inventoryService.updateInventory(this.updateModel()).subscribe({
      next: (response: any) => {
        alert("Updated Successfully");
        this.loadInventory();
        this.selectedInvetoryId.set(null);
        this.closePopup();
      },
      error: (error) => {
        console.error(error);
      }
    })
  }

  deleteInventory() {
    const inventoryId = this.selectedInvetoryId();
    if (inventoryId == null) {
      return;
    }
    this.inventoryService.deleteInventory(inventoryId).subscribe({
      next: (response: any) => {
        alert("Inventory Deleted");
        this.loadInventory();
        this.closePopup();
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  confirmUpdate(id: number) {
    this.selectedInvetoryId.set(id);
    this.showUpdatePopup.set(true);
  }
  confirmDeactivate(id: number) {
    this.selectedInvetoryId.set(id);
    this.showDeactivatePopup.set(true);
  }
  closeUpdatePopup() {
    this.showUpdatePopup.set(false);
    this.showDeactivatePopup.set(false);
    this.selectedInvetoryId.set(null);
  }
}

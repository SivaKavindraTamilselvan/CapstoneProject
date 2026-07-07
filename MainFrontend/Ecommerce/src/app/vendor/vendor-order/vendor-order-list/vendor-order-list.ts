import { Component, computed, effect, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { VendorOrderService } from '../../../services/vendor-order.Service';
import { VendorOrderFilter } from '../../../models/vendor/vendor-order/vendor-order.filter';
import { PagedResponse } from '../../../models/paged-response.model';
import { OrderItemSummaryModel } from '../../../models/admin/admin-orders/get-items.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DatePipe } from '@angular/common';
import { form, FormField, maxLength, min, pattern } from '@angular/forms/signals';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';

@Component({
  selector: 'app-vendor-order-list',
  imports: [PaginationComponent, MobileCardComponent, DataTableComponent, FilterComponent, FormField, ReactiveFormsModule, FormsModule],
  providers: [DatePipe],
  templateUrl: './vendor-order-list.html',
  styleUrl: './vendor-order-list.css',
})
export class VendorOrderList extends BasePage {

  actions = computed<TableAction<OrderItemSummaryModel>[]>(() => {
    if (this.pageTitle() === 'Product Category List') {
      return [
        {
          label: 'view',
          color: 'green',
          action: 'view',
        },
      ];
    }

    return [
      {
        label: 'View',
        color: 'green',
        action: 'view',
      },
      {
        label: 'Mark as Packed',
        color: 'blue',
        action: 'update',
      }
    ];
  });

  columns: Column[] = [
    {
      key: 'orderItemsId',
      header: 'ID'
    },
    {
      key: 'sku',
      header: 'SKU'
    },
    {
      key: 'productName',
      header: 'Product'
    },
    {
      key: 'quantity',
      header: 'Qty'
    },
    {
      key: 'unitPrice',
      header: 'Unit Price'
    },
    {
      key: 'discount',
      header: 'Discount'
    },
    {
      key: 'itemTotal',
      header: 'Total'
    },
    {
      key: 'orderItemStatus',
      header: 'Status'
    },
    {
      key: 'inventoryCity',
      header: 'City'
    }
  ];

  mobileColumns: Column[] = [
    {
      key: 'orderItemsId',
      header: 'ID'
    },
    {
      key: 'sku',
      header: 'SKU'
    },
    {
      key: 'productName',
      header: 'Product'
    },
    {
      key: 'quantity',
      header: 'Qty'
    },
    {
      key: 'unitPrice',
      header: 'Unit Price'
    },
    {
      key: 'discount',
      header: 'Discount'
    },
    {
      key: 'itemTotal',
      header: 'Total'
    },
    {
      key: 'orderItemStatus',
      header: 'Status'
    },
    {
      key: 'inventoryCity',
      header: 'City'
    }
  ];
  handleAction(event: { type: string; row: OrderItemSummaryModel }) {
    switch (event.type) {
      case 'view':

        break;
      case 'update':
        this.confirmActivate(event.row.orderItemsId);
        break;
    }
  }




  orders = signal<PagedResponse<OrderItemSummaryModel> | null>(null);


  orderNumber = signal<string>('');
  orderStatusId = signal<number | null>(null);
  orderItemStatusId = signal<number | null>(null);
  userId = signal<number | null>(null);

  fromDate = signal<string>('');
  toDate = signal<string>('');

  minAmount = signal<number | null>(null);
  maxAmount = signal<number | null>(null);

  orderFilter = signal(new VendorOrderFilter());

  clearFilterValues(): void {
    this.orderFilter.set(new VendorOrderFilter());
  }



  totalPages = computed(() => this.orders()?.totalPages ?? 1);



  constructor(
    private datePipe: DatePipe,
    private router: ActivatedRoute,
    private route: Router,
    private vendorOrderService: VendorOrderService
  ) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }


  orderStatus = signal<number | null>(null);
  orderItemStatus = signal<number | null>(null);

  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.orderStatus.set(data['order']);
      this.orderItemStatus.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadOrders();
    });
  }


  loadOrders(): void {
    this.buildFilter();

    this.vendorOrderService.getOrders(this.orderFilter()).subscribe({
      next: (response: PagedResponse<OrderItemSummaryModel>) => {
        this.orders.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);

        this.orders.set({
          items: [],
          totalCount: 0,
          pageNumber: this.pageNumber(),
          pageSize: this.pageSize(),
          totalPages: 1
        });
      }
    });
  }

  protected loadData(): void {
    this.loadOrders();
  }


  filterForm = form(this.orderFilter, (path) => {
    pattern(path.orderNumber, /^[A-Za-z0-9-]*$/, { message: 'Order number can contain only letters, numbers, and hyphens.' });
    maxLength(path.orderNumber, 50, { message: 'Order number cannot exceed 50 characters.' });
    min(path.orderStatusId, 1, { message: 'Order status ID must be greater than 0.' });
    min(path.minAmount, 0, { message: 'Minimum amount cannot be negative.' });
    min(path.maxAmount, 0, { message: 'Maximum amount cannot be negative.' });
    min(path.userId, 1, { message: 'User ID must be greater than 0.' });
    min(path.orderItemStatusId, 1, { message: 'Vendor ID must be greater than 0.' });
  });


  private buildFilter() {

    this.orderFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      orderStatusId: this.orderStatus(),
      orderItemStatusId: this.orderItemStatus(),
      orderNumber: this.orderNumber().trim(),
    }));
  }



  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.orderStatusId.set(value ? Number(value) : null);
  }

  onOrderStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.orderItemStatusId.set(value ? Number(value) : null);
  }

  onOrderNumberInput(event: Event): void {
    this.orderNumber.set((event.target as HTMLInputElement).value);
  }

  onUserIdInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.userId.set(value ? Number(value) : null);
  }

  onMinPriceInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.minAmount.set(value ? Number(value) : null);
  }

  onMaxPriceInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.maxAmount.set(value ? Number(value) : null);
  }

  onFromDateInput(event: Event): void {
    this.fromDate.set((event.target as HTMLInputElement).value);
  }

  onToDateInput(event: Event): void {
    this.toDate.set((event.target as HTMLInputElement).value);
  }

  updateOrder() {
    const orderId = this.selectedOrderId();
    if (orderId === null) return;
    this.vendorOrderService.updateOrder(orderId).subscribe({
      next: (response: any) => {
        this.loadOrders();
        this.closePopup();
      }
    })
  }

  selectedOrderId = signal<number | null>(null);
  showDeactivatePopup = signal(false);
  confirmActivate(id: number) {
    this.selectedOrderId.set(id);
    this.showDeactivatePopup.set(true);
  }
  closeUpdatePopup() {
    this.showDeactivatePopup.set(false);
    this.selectedOrderId.set(null);
  }
}
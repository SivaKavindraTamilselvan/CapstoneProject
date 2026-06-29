import { Component, computed, signal } from '@angular/core';
import { Router } from '@angular/router';

import { VendorOrderService } from '../../../services/vendor-order.Service';
import { VendorOrderFilter } from '../../../models/vendor/vendor-order/vendor-order.filter';
import { PagedResponse } from '../../../models/paged-response.model';
import { OrderItemSummaryModel } from '../../../models/admin/admin-orders/get-items.model';

@Component({
  selector: 'app-vendor-order-list',
  templateUrl: './vendor-order-list.html',
  styleUrl: './vendor-order-list.css',
})
export class VendorOrderList {

 
  orders = signal<PagedResponse<OrderItemSummaryModel> | null>(null);


  orderNumber = signal<string>('');
  orderStatusId = signal<number | null>(null);
  orderItemStatusId = signal<number | null>(null);
  userId = signal<number | null>(null);

  fromDate = signal<string>('');
  toDate = signal<string>('');

  minAmount = signal<number | null>(null);
  maxAmount = signal<number | null>(null);

  
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);

  totalPages = computed(() => this.orders()?.totalPages ?? 1);


  filterPanelOpen = signal<boolean>(false);

  constructor(
    private router: Router,
    private vendorOrderService: VendorOrderService
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

 
  loadOrders(): void {
    const filter = this.buildFilter();

    this.vendorOrderService.getOrders(filter).subscribe({
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

  private buildFilter(): VendorOrderFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),

      orderNumber: this.orderNumber() || undefined,
      orderStatusId: this.orderStatusId() ?? undefined,
      orderItemStatusId : this.orderItemStatusId() ?? undefined,

      userId: this.userId() ?? undefined,

      fromDate: this.fromDate() || undefined,
      toDate: this.toDate() || undefined,

      minAmount: this.minAmount() ?? undefined,
      maxAmount: this.maxAmount() ?? undefined
    };
  }

  toggleFilterPanel(): void {
    this.filterPanelOpen.update(v => !v);
  }

  closeFilterPanel(): void {
    this.filterPanelOpen.set(false);
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadOrders();
    this.closeFilterPanel();
  }

  resetFilters(): void {
    this.pageNumber.set(1);

    this.orderNumber.set('');
    this.orderStatusId.set(null);
    this.orderItemStatusId.set(null);
    this.userId.set(null);

    this.fromDate.set('');
    this.toDate.set('');

    this.minAmount.set(null);
    this.maxAmount.set(null);

    this.loadOrders();
    this.closeFilterPanel();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;

    this.pageNumber.set(page);
    this.loadOrders();
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
    this.loadOrders();
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
}
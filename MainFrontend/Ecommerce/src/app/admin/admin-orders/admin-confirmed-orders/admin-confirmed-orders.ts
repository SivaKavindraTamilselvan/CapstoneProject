import { Component, computed, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { OrderModel } from '../../../models/admin/admin-orders/get-order.model';
import { Router } from '@angular/router';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { AdminOrderFilter } from '../../../models/admin/admin-orders/get-order.filter';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-confirmed-orders',
  imports: [DatePipe],
  templateUrl: './admin-confirmed-orders.html',
  styleUrl: './admin-confirmed-orders.css',
})
export class AdminConfirmedOrders {
  orders = signal<PagedResponse<OrderModel> | null>(null);


  orderNumber = signal<string>('');
  orderStatusId = signal<number | null>(null);
  userId = signal<number | null>(null);
  vendorId = signal<number | null>(null);

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
    private adminOrderService: AdminOrderService
  ) { }

  ngOnInit() {
    this.loadOrders();
  }


  loadOrders() {
    this.adminOrderService.getOrders(this.buildFilter()).subscribe({
      next: (response: any) => {
        this.orders.set(response);
        console.log(response);
      },
      error: (error) => {
        console.error(error);

        if (error.status === 404) {
          this.orders.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1
          });
        }
      }
    });
  }


  private buildFilter(): AdminOrderFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),

      orderNumber: this.orderNumber() || undefined,
      orderStatusId: 2,
      userId: this.userId() ?? undefined,
      vendorId: this.vendorId() ?? undefined,

      fromDate: this.fromDate() || undefined,
      toDate: this.toDate() || undefined,

      minAmount: this.minAmount() ?? undefined,
      maxAmount: this.maxAmount() ?? undefined
    };
  }


  toggleFilterPanel(): void {
    this.filterPanelOpen.update(open => !open);
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
    this.userId.set(null);
    this.vendorId.set(null);
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
  onOrderNumberInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.orderNumber.set(value);
  }
  onUserIdInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.userId.set(value ? Number(value) : null);
  }
  onVendorIdInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.vendorId.set(value ? Number(value) : null);
  }
  onMinPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.minAmount.set(v ? Number(v) : null);
  }

  onMaxPriceInput(event: Event): void {
    const v = (event.target as HTMLInputElement).value;
    this.maxAmount.set(v ? Number(v) : null);
  }
  onFromDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.fromDate.set(value);
  }

  onToDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.toDate.set(value);
  }
}

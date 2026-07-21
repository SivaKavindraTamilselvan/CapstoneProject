import { Component, computed, effect, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { AdminOrderFilter } from '../../../models/admin/admin-orders/get-order.filter';
import { DatePipe, DecimalPipe } from '@angular/common';
import { PagedResponse } from '../../../models/paged-response.model';
import { OrderModel } from '../../../models/admin/admin-orders/get-order.model';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { BasePage } from '../../../shared-class/shares-page-class';
import { form, FormField, maxLength, min, pattern } from '@angular/forms/signals';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';

@Component({
  selector: 'app-get-admin-orders',
  imports: [MobileCardComponent, FilterComponent, DataTableComponent, PaginationComponent, FormField, ReactiveFormsModule, FormsModule, HeaderComponent],
  providers: [DatePipe],
  templateUrl: './get-admin-orders.html',
  styleUrl: './get-admin-orders.css',
})
export class GetAdminOrders extends BasePage {

  actions: TableAction<OrderModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    }
  ];

  columns: Column[] = [
    {
      key: 'orderId',
      header: 'ID'
    },
    {
      key: 'orderNumber',
      header: 'Order Number'
    },
    {
      key: 'userName',
      header: 'User'
    },
    {
      key: 'orderStatus',
      header: 'Status'
    },
    {
      key: 'totalProductAmount',
      header: 'Product Amount'
    },
    {
      key: 'totalShippingAmount',
      header: 'Shipping'
    },
    {
      key: 'finalAmount',
      header: 'Final Amount'
    },
    {
      key: 'orderDate',
      header: 'Order Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    }

  ];

  mobileColumns: Column[] = [
    {
      key: 'orderId',
      header: 'ID'
    },
    {
      key: 'orderNumber',
      header: 'Order Number'
    },
    {
      key: 'userName',
      header: 'User'
    },
    {
      key: 'orderStatus',
      header: 'Status'
    },
    {
      key: 'totalProductAmount',
      header: 'Product Amount'
    },
    {
      key: 'totalShippingAmount',
      header: 'Shipping'
    },
    {
      key: 'finalAmount',
      header: 'Final Amount'
    },
    {
      key: 'orderDate',
      header: 'Order Date',
      formatter: (value: string) =>
        this.datePipe.transform(value, 'dd/MM/yyyy')
    }
  ];


  orders = signal<PagedResponse<OrderModel> | null>(null);


  orderNumber = signal<string>('');
  orderStatusId = signal<number | null>(null);
  userId = signal<number | null>(null);
  vendorId = signal<number | null>(null);

  fromDate = signal<string>('');
  toDate = signal<string>('');

  minAmount = signal<number | null>(null);
  maxAmount = signal<number | null>(null);

  orderFilter = signal(new AdminOrderFilter());
  clearFilterValues(): void {
    this.orderStatusId.set(null);
    this.orderFilter.set(new AdminOrderFilter());
  }


  totalPages = computed(() => this.orders()?.totalPages ?? 1);


  constructor(
    private datePipe: DatePipe,
    private router: ActivatedRoute,
    private route: Router,
    private adminOrderService: AdminOrderService
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

  categoryStatus = signal<number | null>(null);
  pageTitle = signal<string | null>(null);
  queryVendor = signal<number | null>(null);

  ngOnInit(): void {

    this.router.data.subscribe(data => {
      this.categoryStatus.set(data['status']);
      this.pageTitle.set(data['title']);
    });

    this.router.queryParams.subscribe(params => {
      this.queryVendor.set(
        params['vendorId'] ? Number(params['vendorId']) : null
      );
      this.loadOrders();
    });

  }

  tableLoading = signal(false);
  loadOrders() {
    this.buildFilter();
    this.tableLoading.set(true);
    this.adminOrderService.getOrders(this.orderFilter()).subscribe({
      next: (response: any) => {
        this.orders.set(response);
        //console.log(response);
        this.tableLoading.set(false);
      },
      error: (error) => {
        //console.error(error);

        if (error.status === 404) {
          this.orders.set({
            items: [],
            totalCount: 0,
            pageNumber: this.pageNumber(),
            pageSize: this.pageSize(),
            totalPages: 1
          });
        }
        this.tableLoading.set(false);
      }
    });
  }

  handleAction(event: { type: string; row: OrderModel }) {
    switch (event.type) {
      case 'view':
        this.viewOrder(event.row.orderId);
        break;
    }
  }

  protected loadData(): void {
    this.loadOrders();
  }

  filterForm = form(this.orderFilter, (path) => {
    pattern(path.orderNumber, /^[A-Za-z0-9-]*$/, { message: 'Order number can contain only letters, numbers, and hyphens.' });
    maxLength(path.orderNumber, 50, { message: 'Order number cannot exceed 50 characters.' });
    min(path.orderStatusId, 1, { message: 'Order status ID must be greater than 0.' });
    min(path.minAmount, 1, { message: 'Minimum amount cannot be negative or 0.' });
    min(path.maxAmount, 1, { message: 'Maximum amount cannot be negative or 0.' });
    min(path.userId, 1, { message: 'User ID must be greater than 0.' });
    min(path.vendorId, 1, { message: 'Vendor ID must be greater than 0.' });
  });


  private buildFilter() {
    if (this.queryVendor() != null) {
      this.orderFilter.update(filter => ({
        ...filter,
        vendorId: this.queryVendor()
      }));
    }
    this.orderFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      orderStatusId: this.categoryStatus() == null ? this.orderStatusId() : this.categoryStatus(),
      orderNumber: this.orderNumber().trim(),
    }));
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value === '') {
      this.orderStatusId.set(null);
    } else {
      this.orderStatusId.set(Number(value));
    }
    this.orderFilter.update(filter => ({
      ...filter,
      orderStatusId: value === '' ? null : Number(value)
    }));
  }

  onFromDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.fromDate.set(value);
    this.orderFilter.update(filter => ({
      ...filter,
      fromDate: value || null
    }));
    this.validateDateRange();
  }

  onToDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.toDate.set(value);
    this.orderFilter.update(filter => ({
      ...filter,
      toDate: value || null
    }));
    this.validateDateRange();
  }

  private readonly MIN_VALID_DATE = '2026-06-01';

  private validateDateRange(): void {
    const from = this.orderFilter().fromDate;
    const to = this.orderFilter().toDate;

    const minDate = this.stripTime(new Date(this.MIN_VALID_DATE));
    const today = this.stripTime(new Date());

    const fromDate = from ? this.stripTime(new Date(from)) : null;
    const toDate = to ? this.stripTime(new Date(to)) : null;

    if (fromDate && fromDate < minDate) {
      this.filterErrorMessage.set('From date cannot be before 01/06/2026.');
      return;
    }

    if (toDate && toDate < minDate) {
      this.filterErrorMessage.set('To date cannot be before 01/06/2026.');
      return;
    }

    if (fromDate && fromDate > today) {
      this.filterErrorMessage.set('From date cannot be in the future.');
      return;
    }

    if (toDate && toDate > today) {
      this.filterErrorMessage.set('To date cannot be in the future.');
      return;
    }

    if (fromDate && toDate && fromDate > toDate) {
      this.filterErrorMessage.set('From date cannot be later than To date.');
      return;
    }

    this.filterErrorMessage.set(null);
  }

  private stripTime(date: Date): Date {
    date.setHours(0, 0, 0, 0);
    return date;
  }

  viewOrder(id: number) {
    this.route.navigate(['/admin/order', id]);
  }
}
import { Component, computed, effect, signal } from '@angular/core';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { CancelOrderModel, CancelSummaryModel, RequestAdminCancelFilter } from '../../../models/user/order/cancel.order.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { PagedResponse } from '../../../models/paged-response.model';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { BasePage } from '../../../shared-class/shares-page-class';
import { form, FormField, maxLength, min, pattern } from '@angular/forms/signals';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-cancelled-order',
  imports: [DataTableComponent, MobileCardComponent, PaginationComponent, HeaderComponent, FilterComponent, FormField, ReactiveFormsModule, FormsModule],
  providers: [DatePipe],
  templateUrl: './admin-cancelled-order.html',
  styleUrl: './admin-cancelled-order.css',
})
export class AdminCancelledOrder extends BasePage {
  constructor(private datePipe: DatePipe, private router: Router, private orderService: AdminOrderService, private route: Router) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }
  protected loadData(): void {
    this.loadCancelOrders();
  }

  ngOnInit(): void {
    this.loadCancelOrders();
  }

  actions: TableAction<CancelSummaryModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    }
  ];
  columns: Column[] = [
    { key: 'cancelId', header: 'ID' },
    { key: 'orderNumber', header: 'Order Number' },
    { key: 'productName', header: 'Product' },
    { key: 'vendorName', header: 'Vendor' },
    { key: 'cancelQuantity', header: 'Qty' },
    { key: 'cancelAmount', header: 'Refund' },
    { key: 'cancelStatus', header: 'Status' },
    { key: 'cancelledDate', header: 'Cancelled On', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') }
  ];

  mobileColumns = [...this.columns];

  cancelOrders = signal<PagedResponse<CancelSummaryModel> | null>(null);

  orderNumber = signal('');
  cancelStatus = signal<string>('');
  productName = signal('');
  vendorName = signal('');

  fromDate = signal('');
  toDate = signal('');

  cancelFilter = signal(new RequestAdminCancelFilter());

  totalPages = computed(() =>
    this.cancelOrders()?.totalPages ?? 1
  );

  loadCancelOrders() {
    this.buildFilter();
    this.orderService.getCancelledOrder(this.cancelFilter()).subscribe({
      next: (response: any) => {
        this.cancelOrders.set(response);
        console.log(response);
      },
      error: error => {
        if (error.status == 404) {
          this.cancelOrders.set({
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
  clearFilterValues(): void {
    this.cancelFilter.set(new RequestAdminCancelFilter());
  }
  filterForm = form(this.cancelFilter, path => {
    min(path.cancelStatusId, 1, { message: 'Cancel Status ID must be greater than 0.' });
    min(path.cancelReasonId, 1, { message: 'Cancel Reason ID must be greater than 0.' });
    min(path.vendorId, 1, { message: 'Vendor ID must be greater than 0.' });
    min(path.orderId, 1, { message: 'Order ID must be greater than 0.' });
    min(path.orderItemId, 1, { message: 'Order Item ID must be greater than 0.' });
    min(path.productVariantId, 1, { message: 'Product Variant ID must be greater than 0.' });
  });
  private buildFilter() {
    this.cancelFilter.update(filter => ({
      ...filter,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      fromDate: this.fromDate() || null,
      toDate: this.toDate() || null
    }));

  }

  onFromDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.fromDate.set(value);
    this.cancelFilter.update(filter => ({
      ...filter,
      fromDate: value || null
    }));
  }

  onToDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.toDate.set(value);
    this.cancelFilter.update(filter => ({
      ...filter,
      toDate: value || null
    }));
  }
  viewCancel(id: number) {
    this.route.navigate(['admin/orders/cancelled-order', id]);
  }
  handleAction(event: { type: string; row: CancelSummaryModel }) {
    switch (event.type) {
      case 'view':
        this.viewCancel(event.row.cancelId);
        break;

    }
  }
}

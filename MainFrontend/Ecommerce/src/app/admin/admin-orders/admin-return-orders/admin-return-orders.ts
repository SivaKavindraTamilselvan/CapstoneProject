import { Component, computed, effect, signal, ViewChild } from '@angular/core';
import { BasePage } from '../../../shared-class/shares-page-class';
import { DatePipe, NgClass } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { ReturnSummaryModel } from '../../../models/user/order/return.order.model';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { PagedResponse } from '../../../models/paged-response.model';
import { RequestAdminReturnFilter } from '../../../models/admin/admin-orders/get-order.model';
import { form, FormField, min } from '@angular/forms/signals';
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FilterComponent } from '../../../shared-components/filter-component/filter-component';
import { ReturnRefundPopup } from '../return-refund-popup/return-refund-popup';
import { ReturnListModel } from '../../../models/vendor/vendor-return/return.model';

@Component({
  selector: 'app-admin-return-orders',
  imports: [HeaderComponent, DataTableComponent, MobileCardComponent, PaginationComponent, NgClass, FormsModule, ReactiveFormsModule, FormField, FilterComponent, ReturnRefundPopup],
  providers: [DatePipe],
  templateUrl: './admin-return-orders.html',
  styleUrl: './admin-return-orders.css',
})
export class AdminReturnOrders extends BasePage {
  constructor(private datePipe: DatePipe, private router: Router, private orderService: AdminOrderService, private route: ActivatedRoute) {
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
    this.loadReturnOrders();
  }

  categoryStatus = signal<number | null>(null);
  pageTitle = signal<string | null>(null);
  ongoing = signal<boolean | null>(null);
  queryVendor = signal<number | null>(null);


  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.categoryStatus.set(data['status']);
      this.ongoing.set(data['ongoing']);
      this.pageTitle.set(data['title']);
    });
    this.loadReturnOrders();
  }

  actions: TableAction<ReturnSummaryModel>[] = [
    {
      label: 'View',
      color: 'green',
      action: 'view'
    },
    {
      label: 'Update',
      color: 'blue',
      action: 'update'
    }
  ];

  columns: Column[] = [
    { key: 'returnId', header: 'ID' },
    { key: 'productName', header: 'Product' },
    { key: 'sku', header: 'SKU' },
    { key: 'vendorName', header: 'Vendor' },
    { key: 'returnQuantity', header: 'Qty' },
    { key: 'returnAmount', header: 'Amount' },
    { key: 'returnStatus', header: 'Status' },
    { key: 'requestedDate', header: 'Requested On', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') }
  ];

  mobileColumns = [...this.columns];

  returnOrders = signal<PagedResponse<ReturnListModel> | null>(null);

  fromDate = signal('');
  toDate = signal('');

  // --- Filter panel state (matching vendor return filter pattern) ---
  draftstatus = signal<number | null>(null);
  returnReasonId = signal<number | null>(null);

 
  returnFilter = signal(new RequestAdminReturnFilter());

  totalPages = computed(() =>
    this.returnOrders()?.totalPages ?? 1
  );

  tableLoading = signal(false);
  loadReturnOrders() {
    this.buildFilter();
    this.tableLoading.set(true);
    this.orderService.getReturnedOrder(this.returnFilter()).subscribe({
      next: (response: any) => {
        this.tableLoading.set(false);
        this.returnOrders.set(response);
      },
      error: error => {
        if (error.status == 404) {
          this.returnOrders.set({
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

  clearFilterValues(): void {
    this.draftstatus.set(null);
    this.returnReasonId.set(null);
    this.fromDate.set('');
    this.toDate.set('');
    this.returnFilter.set(new RequestAdminReturnFilter());
    this.returnFilter.update(filter => ({
      ...filter,
      returnReasonId: null,
      returnStatusId: null
    }));
  }

  filterForm = form(this.returnFilter, path => {
    min(path.returnStatusId, 1, { message: 'Return Status ID must be greater than 0.' });
    min(path.returnReasonId, 1, { message: 'Return Reason ID must be greater than 0.' });
    min(path.vendorId, 1, { message: 'Vendor ID must be greater than 0.' });
    min(path.orderId, 1, { message: 'Order ID must be greater than 0.' });
    min(path.orderItemId, 1, { message: 'Order Item ID must be greater than 0.' });
  });

  private buildFilter() {
    if (this.categoryStatus() != null) {
      this.returnFilter.update(filter => ({
        ...filter,
        returnStatusId: this.categoryStatus()
      }));
    } else {
      this.returnFilter.update(filter => ({
        ...filter,
        returnStatusId: this.draftstatus()
      }));
    }
    if (this.categoryStatus() == null && this.ongoing() == true) {
      this.returnFilter.update(filter => ({
        ...filter,
        onGoing: this.ongoing()
      }));
    }
    this.returnFilter.update(filter => ({
      ...filter,
      returnReasonId: this.returnReasonId(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      fromDate: this.fromDate() || null,
      toDate: this.toDate() || null,
    }));
  }


  onFromDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.fromDate.set(value);
    this.returnFilter.update(filter => ({
      ...filter,
      fromDate: value || null
    }));
    this.validateDateRange();
  }

  onToDateInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.toDate.set(value);
    this.returnFilter.update(filter => ({
      ...filter,
      toDate: value || null
    }));
    this.validateDateRange();
  }

  onReturnReasonChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.returnReasonId.set(value ? Number(value) : null);
  }

  onReturnStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.draftstatus.set(value ? Number(value) : null);
  }

  private readonly MIN_VALID_DATE = '2026-06-01';

  private validateDateRange(): void {
    const from = this.returnFilter().fromDate;
    const to = this.returnFilter().toDate;

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

  viewReturn(id: number) {
    this.router.navigate(['admin/order/order-item', id]);
  }


  @ViewChild(ReturnRefundPopup) refundPopup!: ReturnRefundPopup;


  createRefund(returnId: number, returnAmount: number) {
    this.refundPopup.openPopup(returnId, returnAmount);
  }

  handleAction(event: { type: string; row: ReturnSummaryModel }) {
    switch (event.type) {
      case 'view':
        this.viewReturn(event.row.orderItemId);
        break;
      case 'update':
        this.createRefund(event.row.returnId, event.row.returnAmount);
        break;
    }
  }
}
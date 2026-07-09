import { Component, computed, effect, signal } from '@angular/core';
import { BasePage } from '../../../shared-class/shares-page-class';
import { Column } from '../../../shared-components/data-table-component/column.model';
import { DatePipe } from '@angular/common';
import { TableAction } from '../../../shared-components/data-table-component/table-actions.model';
import { ReturnListModel } from '../../../models/vendor/vendor-return/return.model';
import { ActivatedRoute, Router } from '@angular/router';
import { VendorOrderService } from '../../../services/vendor-order.Service';
import { form, min } from '@angular/forms/signals';
import { RequestVendorReturnFilter } from '../../../models/vendor/vendor-order/vendor-order.filter';
import { PagedResponse } from '../../../models/paged-response.model';

@Component({
  selector: 'app-vendor-return-order',
  imports: [],
  providers: [DatePipe],
  templateUrl: './vendor-return-order.html',
  styleUrl: './vendor-return-order.css',
})
export class VendorReturnOrder extends BasePage {

  constructor(private datePipe: DatePipe, private route: Router, private router: ActivatedRoute, private vendorOrderService: VendorOrderService) {
    super();
    effect(() => {
      if (this.filterForm().invalid()) {
        this.filterErrorMessage.set('Please fix the validation errors.');
      } else {
        this.filterErrorMessage.set(null);
      }
    });
  }

  status = signal<number | null>(null);
  draftstatus = signal<number | null>(null);
  pageTitle = signal<string | null>(null);

  ngOnInit(): void {
    this.router.data.subscribe(data => {
      this.status.set(data['status']);
      this.pageTitle.set(data['title']);
      this.loadReturn();
    });
  }

  protected loadData(): void {
    this.loadReturn();
  }

  returns = signal<PagedResponse<ReturnListModel> | null>(null);
  requestVendorReturnFilter = signal(new RequestVendorReturnFilter());
  totalPages = computed(() => this.returns()?.totalPages ?? 1);

  filterForm = form(this.requestVendorReturnFilter, (path) => {
    min(path.returnStatusId, 1, { message: 'Please select a valid return status.' });
    min(path.returnReasonId, 1, { message: 'Please select a valid return reason.' });
    min(path.orderItemId, 1, { message: 'Order Item ID must be greater than 0.' });
    min(path.orderId, 1, { message: 'Order ID must be greater than 0.' });
  });

  private buildFilters() {
    this.requestVendorReturnFilter.update(filter => ({
      ...filter,
      approvalStatusId: this.draftstatus != null && this.status() != 4 && this.status() != 1 ? this.draftstatus() : this.status(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    }));
  }

  clearFilterValues(): void {
      this.draftstatus.set(null);
      this.requestVendorReturnFilter.set(new RequestVendorReturnFilter());
      this.requestVendorReturnFilter.update(filter => ({
        ...filter,
        approvalStatusId: null,
        isActive: null,
      }));
    }
  

  successMessage = signal<string>('');
  errorMessage = signal<string>('');
  progress = signal(false);

  

  loadReturn() {
    this.buildFilters();
    this.vendorOrderService.getReturnOrder(this.requestVendorReturnFilter()).subscribe({
      next: (response: any) => {
        this.returns.set(response);
        console.log(this.returns());
      },
      error: (error) => {
        console.log(error);
        if (error.status === 404) {
          this.returns.set({
            items: [],
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0,
          });
        } else if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors).flat().join(', ');
          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(error.errorMessage ?? '');
        }
      },
    });
  }

  columns: Column[] = [
    { key: 'returnId', header: 'Return ID' },
    { key: 'productImageUrl', header: 'Image' },
    { key: 'productName', header: 'Product' },
    { key: 'sku', header: 'SKU' },
    { key: 'vendorName', header: 'Vendor' },
    { key: 'returnQuantity', header: 'Return Qty' },
    { key: 'returnAmount', header: 'Return Amount', formatter: (value: number) => `₹${value}` },
    { key: 'returnReason', header: 'Reason' },
    { key: 'returnStatus', header: 'Status' },
    { key: 'inventoryCity', header: 'Inventory City' },
    { key: 'deliveryCity', header: 'Delivery City' },
    { key: 'requestedDate', header: 'Requested On', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') ?? '' },
    { key: 'reviewedDate', header: 'Reviewed On', formatter: (value: string | null) => value ? this.datePipe.transform(value, 'dd/MM/yyyy') ?? '' : '-' }
  ];

  mobileColumns = [...this.columns];

  actions: TableAction<ReturnListModel>[] = [
    { label: 'View', color: 'green', action: 'view' }
  ];

  handleAction(event: { type: string; row: ReturnListModel }) {
    switch (event.type) {
      case 'view':
        this.viewReturn(event.row.returnId);
        break;
    }
  }

  viewReturn(returnId: number) {
    this.route.navigate(['/vendor/return-details', returnId]);
  }

}
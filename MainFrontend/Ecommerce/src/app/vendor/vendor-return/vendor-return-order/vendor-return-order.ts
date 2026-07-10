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
import { HeaderComponent } from '../../../shared-components/header-component/header-component';
import { PaginationComponent } from '../../../shared-components/pagination-component/pagination-component';
import { DataTableComponent } from '../../../shared-components/data-table-component/data-table-component';
import { MobileCardComponent } from '../../../shared-components/mobile-card-component/mobile-card-component';
import { ReviewReturnComponent } from '../review-return-component/review-return-component';
import { AdditionalRefundPopup } from '../additional-refund-popup/additional-refund-popup';
import { ReviewReturnProductPopup } from '../review-return-product/review-return-product';

@Component({
  selector: 'app-vendor-return-order',
  imports: [HeaderComponent, PaginationComponent, DataTableComponent, MobileCardComponent, ReviewReturnComponent, AdditionalRefundPopup, ReviewReturnProductPopup],
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
      returnStatusId: this.status() == null ? this.draftstatus() : this.status(),
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
    { key: 'sku', header: 'SKU' },
    { key: 'returnQuantity', header: 'Qty' },
    { key: 'returnAmount', header: 'Amount', formatter: (value: number) => `₹${value}` },
    { key: 'inventoryCity', header: 'Inventory City' },
    { key: 'deliveryCity', header: 'Delivery City' },
    { key: 'requestedDate', header: 'Requested On', formatter: (value: string) => this.datePipe.transform(value, 'dd/MM/yyyy') ?? '' },
  ];

  mobileColumns = [...this.columns];

  actions = computed<TableAction<ReturnListModel>[]>(() => {
    if (this.status() == 1) {
      return [
        { label: 'View', color: 'green', action: 'view' },
        { label: 'Approve', color: 'green', action: 'approve' },
        { label: 'Reject', color: 'red', action: 'reject' },
        { label: 'Review', color: 'gray', action: 'review' },
      ];
    }
    else if (this.status() == 6) {
      return [
        { label: 'View', color: 'green', action: 'view' },
        { label: 'Review', color: 'gray', action: 'review' },
      ];
    }
    else if (this.status() == 8) {
      return [
        { label: 'View', color: 'green', action: 'view' },
        { label: 'Review', color: 'gray', action: 'review' },
      ];
    }

    return [
      { label: 'Accept & Refund', color: 'green', action: 'refund' }
    ];
  });

  handleAction(event: { type: string; row: ReturnListModel }) {
    switch (event.type) {
      case 'view':
        this.viewReturn(event.row.returnId);
        break;
      case 'approve':
        this.openApprovePopup(event.row.returnId);
        break;
      case 'reject':
        this.openRejectPopup(event.row.returnId);
        break;
      case 'review':
        this.openReviewProductPopup(event.row.returnId);
        break;
    }
  }
  showReviewPopup = signal(false);
  selectedReviewReturnId = signal<number | null>(null);
  reviewDecision = signal<boolean>(true);

  showReviewProductPopup = signal(false);
  selectedProductReturnId = signal<number | null>(null);

  showAdditionalRefundPopup = signal(false);
  selectedAdditionalRefundReturnId = signal<number | null>(null);

  openApprovePopup(returnId: number) {
    this.selectedReviewReturnId.set(returnId);
    this.reviewDecision.set(true);
    this.showReviewPopup.set(true);
  }

  openRejectPopup(returnId: number) {
    this.selectedReviewReturnId.set(returnId);
    this.reviewDecision.set(false);
    this.showReviewPopup.set(true);
  }

  closeReviewPopup() {
    this.showReviewPopup.set(false);
    this.selectedReviewReturnId.set(null);
  }

  onReviewed() {
    this.loadReturn();
  }

  openReviewProductPopup(returnId: number) {
    this.selectedProductReturnId.set(returnId);
    this.showReviewProductPopup.set(true);
  }

  closeReviewProductPopup() {
    this.showReviewProductPopup.set(false);
    this.selectedProductReturnId.set(null);
  }

  onAccepted() {
    this.loadReturn();
  }

  onRequestAdditionalRefund(returnId: number) {
    this.selectedAdditionalRefundReturnId.set(returnId);
    this.showAdditionalRefundPopup.set(true);
  }

  closeAdditionalRefundPopup() {
    this.showAdditionalRefundPopup.set(false);
    this.selectedAdditionalRefundReturnId.set(null);
  }

  onAdditionalRefundSubmitted() {
    this.loadReturn();
  }

  viewReturn(returnId: number) {
    this.route.navigate(['/vendor/return-details', returnId]);
  }

}
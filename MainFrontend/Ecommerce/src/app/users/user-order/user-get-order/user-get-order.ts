import { CommonModule } from '@angular/common';
import { Component, computed, effect, signal } from '@angular/core';
import { PagedResponse } from '../../../models/paged-response.model';
import { OrderModel } from '../../../models/admin/admin-orders/get-order.model';
import { Router } from '@angular/router';
import { UserOrderService } from '../../../services/user-order.Service';
import { UserOrderFilter } from '../../../models/user/order/order-fiter';
import { CancelOrderModel } from '../../../models/user/order/cancel.order.model';
import { form, FormField, pattern, required } from '@angular/forms/signals';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddReturnModel } from '../../../models/user/order/return.order.model';
import { AddReturnComponent } from '../add-return-component/add-return-component';

@Component({
  selector: 'app-user-get-order',
  imports: [CommonModule, FormField, ReactiveFormsModule, FormsModule,AddReturnComponent],
  templateUrl: './user-get-order.html',
  styleUrl: './user-get-order.css',
})
export class UserGetOrder {
  orders = signal<PagedResponse<OrderModel> | null>(null);


  showActivatePopup = signal(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);
  selectedOrderId = signal<number | null>(null);


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
  constructor(private router: Router, private userOrderService: UserOrderService) {

  }
  ngOnInit() {
    this.loadOrders();
  }
  loadOrders() {
    this.userOrderService.getOrders(this.buildFilter()).subscribe({
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


  private buildFilter(): UserOrderFilter {
    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),

      orderNumber: this.orderNumber() || undefined,
      orderStatusId: this.orderStatusId() ?? undefined,

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
    this.orderStatusId.set(null);
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
  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value === '') {
      this.orderStatusId.set(null);
    } else {
      this.orderStatusId.set(Number(value));
    }
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
  expandedOrderId = signal<number | null>(null);

  toggleExpand(orderId: number) {
    this.expandedOrderId.set(
      this.expandedOrderId() === orderId ? null : orderId
    );
  }

  isExpanded(orderId: number) {
    return this.expandedOrderId() === orderId;
  }

  statusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'delivered':
      case 'refunded': return 'bg-green-100 text-green-700';
      case 'processed': return 'bg-blue-100 text-blue-700';
      case 'cancelled': return 'bg-red-100 text-red-700';
      case 'pending':
      case 'packed': return 'bg-yellow-100 text-yellow-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  }



  openCancelPopup(orderItemId: number) {
    this.cancelModel.set(
      new CancelOrderModel(
        0,              // cancelReasonId
        orderItemId,    // orderItemId
        1,              // cancelStatusId
        '',             // additionalReason
        1               // cancelQuantity
      )
    );

    this.showActivatePopup.set(true);
  }


  closePopup() {
    this.showActivatePopup.set(false);
    this.selectedOrderId.set(null);
    this.cancelModel.set(new CancelOrderModel());
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  cancelModel = signal(new CancelOrderModel());

  cancelForm = form(this.cancelModel, (path) => {
    required(path.cancelReasonId, {
      message: 'Select the cancel reason'
    });

    required(path.cancelStatusId, {
      message: 'Select the cancel status'
    });

    required(path.cancelQuantity, {
      message: 'Enter the cancel quantity'
    });

    required(path.additionalReason, {
      message: 'Enter the additional reason'
    });
  });
  handleCancel() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.cancelForm().invalid()) {
      this.errorMessage.set('Enter proper details');
      return;
    }

    const request = {
      cancelReasonId: Number(this.cancelModel().cancelReasonId),
      orderItemId: this.cancelModel().orderItemId,
      cancelStatusId: Number(this.cancelModel().cancelStatusId),
      additionalReason: this.cancelModel().additionalReason,
      cancelQuantity: Number(this.cancelModel().cancelQuantity)
    };

    this.userOrderService.cancelOrder(request).subscribe({
      next: () => {
        this.successMessage.set('Order cancelled successfully');

        setTimeout(() => {
          this.closePopup();
          this.loadOrders();
        }, 2000);
      },
      error: (error) => {
        if (error.status === 400 && error.error?.errors) {
          const messages = Object.values(error.error.errors)
            .flat()
            .join(', ');

          this.errorMessage.set(messages);
        } else {
          this.errorMessage.set(
            error.error?.message ?? 'Something went wrong. Please try again.'
          );
        }
      }
    });
  }
  onReasonChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.cancelForm.cancelReasonId().value.set(value ? Number(value) : 0);
  }

  showReturnPopup = signal(false);
  selectedReturnOrderItemId = signal<number | null>(null);

  openReturnPopup(orderItemId: number) {
    this.selectedReturnOrderItemId.set(orderItemId);
    this.showReturnPopup.set(true);
  }

  closeReturnPopup() {
    this.showReturnPopup.set(false);
    this.selectedReturnOrderItemId.set(null);
  }

  onReturnSubmitted() {
    this.loadOrders();
  }


  goToOrderDetails(orderId: number): void {
    this.router.navigate(['/user/orders', orderId]);
  }
}

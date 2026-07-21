import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserOrderService } from '../../../services/user-order.Service';
import { ShipmentModel } from '../../../models/admin/admin-shipment/admin-shipment.model';
import { OrderItemModel } from '../../../models/admin/admin-orders/get-orderitem.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { CreateReviewComponent } from '../create-review-component/create-review-component';

@Component({
  selector: 'app-user-order-details',
  imports: [DatePipe, NgClass, DecimalPipe, CreateReviewComponent],
  templateUrl: './user-order-details.html',
  styleUrl: './user-order-details.css',
})
export class UserOrderDetails implements OnInit {

  orderItem = signal<OrderItemModel | null>(null);
  shipment = signal<ShipmentModel | null>(null);
  loading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: UserOrderService
  ) { }

  ngOnInit(): void {

    const orderItemsId = Number(this.route.snapshot.paramMap.get('id'));

    if (!orderItemsId) {
      this.errorMessage.set('Invalid Order Item Id');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);

    this.orderService.getOrdersDetails(orderItemsId).subscribe({
      next: (response: OrderItemModel) => {
        this.orderItem.set(response);
        console.log(this.orderItem());
      },
      error: () => {
        this.errorMessage.set('Unable to load order item details.');
      }
    });

    this.orderService.getShipmentDetails(orderItemsId).subscribe({
      next: (response: any) => {
        this.shipment.set(response);
        this.loading.set(false);
      },
      error: () => {
        this.shipment.set(null);
        this.loading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/user/orders']);
  }

  showReviewPopup = signal(false);
  selectedOrderDetailsId = signal(0);

  openReviewPopup(orderDetailsId: number) {
    this.selectedOrderDetailsId.set(orderDetailsId);
    this.showReviewPopup.set(true);
  }

  closePopup() {
    this.showReviewPopup.set(false);
  }

  openCancelPopup(orderItemsId: number): void {
    // wire up your existing cancel modal logic
  }

  statusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'bg-amber-100 text-amber-700';
      case 'Confirmed':
      case 'Payment_Success': return 'bg-blue-100 text-blue-700';
      case 'Shipped': return 'bg-indigo-100 text-indigo-700';
      case 'Delivered':
      case 'Completed': return 'bg-green-100 text-green-700';
      case 'Cancelled':
      case 'Failed': return 'bg-red-100 text-red-700';
      default: return 'bg-slate-100 text-slate-700';
    }
  }
  returnStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'approved':
      case 'refunded': return 'bg-green-100 text-green-700';
      case 'requested':
      case 'pending': return 'bg-yellow-100 text-yellow-700';
      case 'rejected': return 'bg-red-100 text-red-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  }
  downloadInvoice(orderId: number): void {
    this.orderService.downloadInvoice(orderId).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Invoice-Order-${orderId}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Failed to download invoice', error);
      }
    });
  }
}
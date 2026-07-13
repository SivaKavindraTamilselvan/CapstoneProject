import { Component, signal } from '@angular/core';
import { OrderModel } from '../../../models/admin/admin-orders/get-order.model';
import { VendorOrderService } from '../../../services/vendor-order.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderItemModel } from '../../../models/admin/admin-orders/get-orderitem.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { CancelSummaryModel, RequestAdminCancelFilter } from '../../../models/user/order/cancel.order.model';
import { AdminOrderService } from '../../../services/admin-order.Service';

@Component({
  selector: 'app-vendor-order-details',
  imports: [DecimalPipe, NgClass, DatePipe],
  templateUrl: './vendor-order-details.html',
  styleUrl: './vendor-order-details.css',
})
export class VendorOrderDetails {
  orderItem = signal<OrderItemModel | null>(null);



  constructor(private vendorOrderService: VendorOrderService, private adminService: AdminOrderService, private route: ActivatedRoute, private router: Router) {

  }
  ngOnInit(): void {
    const orderid = Number(this.route.snapshot.paramMap.get('id'));
    if (orderid) {
      this.loadOrderDetails(orderid);
    }
  }
  loadOrderDetails(productId: number) {
    this.vendorOrderService.getOrdersDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.orderItem.set(response);
        const id = this.orderItem()?.orderItemsId;
        if (id == null) {
          return;
        }
        if (this.orderItem()?.orderItemStatus == "Cancelled") {
          //this.loadCancelDetail(id);
        }
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
  cancelDetail = signal<CancelSummaryModel | null>(null);
  loading = signal(false);
  loadCancelDetail(cancelId: number) {
    alert(cancelId);
    this.loading.set(true);
    var filter = new RequestAdminCancelFilter();
    filter.orderItemId = cancelId;
    this.adminService.getCancelledOrder(filter).subscribe({
      next: (response: any) => {
        this.cancelDetail.set(response);
        this.loading.set(false);
        console.log(response);
      },
      error: (error) => {
        console.log(error);
        this.loading.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/vendor/orders/list']);
  }
  viewVariant(id: number) {
    this.router.navigate(['/vendor/products/variant', id]);
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Approved':
        return 'bg-green-100 text-green-700 border border-green-300';
      case 'Rejected':
        return 'bg-red-100 text-red-700 border border-red-300';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-700 border border-yellow-300';
      default:
        return 'bg-gray-100 text-gray-700 border border-gray-300';
    }
  }
}


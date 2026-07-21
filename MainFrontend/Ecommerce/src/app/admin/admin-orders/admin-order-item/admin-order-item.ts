import { Component, signal } from '@angular/core';
import { OrderItemModel } from '../../../models/admin/admin-orders/get-orderitem.model';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { ActivatedRoute, Router } from '@angular/router';
import { CancelSummaryModel, RequestAdminCancelFilter } from '../../../models/user/order/cancel.order.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-order-item',
  imports: [DatePipe,DecimalPipe,NgClass],
  templateUrl: './admin-order-item.html',
  styleUrl: './admin-order-item.css',
})
export class AdminOrderItem {
   orderItem = signal<OrderItemModel | null>(null);
   orderId = signal<number>(0);



  constructor(private orderService: AdminOrderService, private route: ActivatedRoute, private router: Router) {

  }
  ngOnInit(): void {
    const orderid = Number(this.route.snapshot.paramMap.get('id'));
    this.orderId.set(orderid);
    if (orderid) {
      this.loadOrderDetails(orderid);
    }
  }
  tableLoading = signal(false);
  loadOrderDetails(productId: number) {
    this.tableLoading.set(true);
    this.orderService.getOrdersItemsDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.orderItem.set(response);
        this.tableLoading.set(false);
        const id = this.orderItem()?.orderItemsId;
        if (id == null) {
          return;
        }
        if (this.orderItem()?.orderItemStatus == "Cancelled") {
          //this.loadCancelDetail(id);
        }
      },
      error: (error) => {
        //console.error(error);
        this.tableLoading.set(false);
      }
    })
  }
  cancelDetail = signal<CancelSummaryModel | null>(null);
  loading = signal(false);
  loadCancelDetail(cancelId: number) {
    //alert(cancelId);
    this.loading.set(true);
    var filter = new RequestAdminCancelFilter();
    filter.orderItemId = cancelId;
    this.orderService.getCancelledOrder(filter).subscribe({
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
    this.router.navigate(['/admin/order',this.orderItem()?.orderId]);
  }
  viewVariant(id: number) {
    this.router.navigate(['/admin/product-variant-details', id]);
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



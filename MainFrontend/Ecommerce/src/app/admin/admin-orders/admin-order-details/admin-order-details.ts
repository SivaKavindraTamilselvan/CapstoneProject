import { Component, signal } from '@angular/core';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute } from '@angular/router';
import { AdminOrderService } from '../../../services/admin-order.Service';
import { OrderModel } from '../../../models/admin/admin-orders/get-order.model';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { OrderItemSummaryModel } from '../../../models/admin/admin-orders/get-items.model';

@Component({
  selector: 'app-admin-order-details',
  imports: [DecimalPipe,NgClass,DatePipe],
  templateUrl: './admin-order-details.html',
  styleUrl: './admin-order-details.css',
})
export class AdminOrderDetails {
  orderModel = signal<OrderModel| null>(null);


  constructor(private orderService: AdminOrderService, private route: ActivatedRoute) {

  }
  ngOnInit(): void {
    const orderid = Number(this.route.snapshot.paramMap.get('id'));
    if (orderid) {
      this.loadOrderDetails(orderid);
    }
  }
  loadOrderDetails(productId: number) {
    this.orderService.getOrdersDetails(productId).subscribe({
      next: (response: any) => {
        console.log(response);
        this.orderModel.set(response);
      },
      error: (error) => {
        console.error(error);
      }
    })
  }
}

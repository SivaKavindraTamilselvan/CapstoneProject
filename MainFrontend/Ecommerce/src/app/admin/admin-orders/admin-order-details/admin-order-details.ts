import { Component, signal } from '@angular/core';
import { ProductModel } from '../../../models/product/product.model';
import { ActivatedRoute, Router } from '@angular/router';
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


  constructor(private orderService: AdminOrderService, private route: ActivatedRoute,private router : Router) {

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
  goBack(){
    this.router.navigate(['/admin/orders/list']);
  }
  viewVariant(id : number){
    this.router.navigate(['/admin/product-variant-details', id]);
  }
  viewCancel(id : number){
    this.router.navigate(['/admin/orders/cancelled-order', id]);
  }
  viewShipment(id : number){
    this.router.navigate(['/admin/shipments/list'],
       {
        queryParams:
        {
          orderId: id
        }
      }
    );
  }
}

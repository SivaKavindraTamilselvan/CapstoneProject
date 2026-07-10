import { Component, signal } from '@angular/core';
import { OrderModel } from '../../../models/admin/admin-orders/get-order.model';

@Component({
  selector: 'app-vendor-order-details',
  imports: [],
  templateUrl: './vendor-order-details.html',
  styleUrl: './vendor-order-details.css',
})
export class VendorOrderDetails {
  orderModel = signal<OrderModel| null>(null);
}


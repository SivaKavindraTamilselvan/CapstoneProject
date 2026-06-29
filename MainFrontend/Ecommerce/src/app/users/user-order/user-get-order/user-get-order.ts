import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';

interface OrderItem {
  orderItemsId: number;
  sku: string;
  productName: string;
  vendorName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  inventoryId: number;
  inventoryCity: string;
  inventoryAddress: string;
  itemTotal: number;
  orderItemStatus: string;
}
 
interface Order {
  orderId: number;
  orderNumber: string;
  userName: string;
  totalProductAmount: number;
  totalShippingAmount: number;
  totalCouponAmount: number;
  finalAmount: number;
  orderStatus: string;
  orderDate: string;
  totalItems: number;
  orderItems: OrderItem[];
}

@Component({
  selector: 'app-user-get-order',
  imports: [CommonModule],
  templateUrl: './user-get-order.html',
  styleUrl: './user-get-order.css',
})
export class UserGetOrder {
  selectedOrder = signal<Order | null>(null);
 
  orders = signal<Order[]>([
    {
      orderId: 20,
      orderNumber: 'ORD-20260628-528973',
      userName: 'Pranesh T',
      totalProductAmount: 79999,
      totalShippingAmount: 159.71,
      totalCouponAmount: 0.0,
      finalAmount: 80158.71,
      orderStatus: 'Failed',
      orderDate: '2026-06-28T22:05:27.179109',
      totalItems: 1,
      orderItems: [{
        orderItemsId: 32,
        sku: 'PV-000006-8B505E82',
        productName: 'iPhone 15',
        vendorName: 'SameenaTextiles',
        quantity: 1,
        unitPrice: 79999,
        discount: 0.0,
        inventoryId: 3,
        inventoryCity: 'Hyderabad',
        inventoryAddress: '22 Banjara Hills Road No. 3',
        itemTotal: 79999.0,
        orderItemStatus: 'Pending'
      }]
    },
    {
      orderId: 17,
      orderNumber: 'ORD-20260628-330644',
      userName: 'Pranesh T',
      totalProductAmount: 79999,
      totalShippingAmount: 159.71,
      totalCouponAmount: 100,
      finalAmount: 80058.71,
      orderStatus: 'Confirmed',
      orderDate: '2026-06-28T22:00:15.470855',
      totalItems: 1,
      orderItems: [{
        orderItemsId: 29,
        sku: 'PV-000006-8B505E82',
        productName: 'iPhone 15',
        vendorName: 'SameenaTextiles',
        quantity: 1,
        unitPrice: 79999,
        discount: 0.0,
        inventoryId: 3,
        inventoryCity: 'Hyderabad',
        inventoryAddress: '22 Banjara Hills Road No. 3',
        itemTotal: 79999.0,
        orderItemStatus: 'Pending'
      }]
    },
    {
      orderId: 14,
      orderNumber: 'ORD-20260628-634954',
      userName: 'Pranesh T',
      totalProductAmount: 79999,
      totalShippingAmount: 159.71,
      totalCouponAmount: 0.0,
      finalAmount: 80158.71,
      orderStatus: 'Pending',
      orderDate: '2026-06-28T21:56:19.506364',
      totalItems: 1,
      orderItems: [{
        orderItemsId: 26,
        sku: 'PV-000006-8B505E82',
        productName: 'iPhone 15',
        vendorName: 'SameenaTextiles',
        quantity: 1,
        unitPrice: 79999,
        discount: 0.0,
        inventoryId: 3,
        inventoryCity: 'Hyderabad',
        inventoryAddress: '22 Banjara Hills Road No. 3',
        itemTotal: 79999.0,
        orderItemStatus: 'Pending'
      }]
    }
  ]);
 
  readonly ORDER_STEPS = ['Pending', 'Confirmed', 'Packed', 'Shipped', 'Out for Delivery'];
 
  getStepIndex(status: string): number {
    const map: Record<string, number> = {
      'Pending': 0,
      'Confirmed': 1,
      'Packed': 2,
      'Shipped': 3,
      'Out for Delivery': 4,
      'Delivered': 4
    };
    return map[status] ?? 0;
  }
 
  isFailed(order: Order): boolean {
    return order.orderStatus === 'Failed';
  }
 
  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Confirmed': 'status-confirmed',
      'Pending': 'status-pending',
      'Failed': 'status-failed',
      'Delivered': 'status-delivered'
    };
    return map[status] ?? 'status-pending';
  }
 
  getItemStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'item-status-pending',
      'Packed': 'item-status-packed',
      'Shipped': 'item-status-shipped',
      'Delivered': 'item-status-delivered',
      'Cancelled': 'item-status-cancelled'
    };
    return map[status] ?? 'item-status-pending';
  }
 
  openOrder(order: Order): void {
    this.selectedOrder.set(order);
  }
 
  closeOrder(): void {
    this.selectedOrder.set(null);
  }
 
  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }
 
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency', currency: 'INR', maximumFractionDigits: 2
    }).format(amount);
  }
}

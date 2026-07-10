import { Component, computed, inject, signal } from '@angular/core';
import { AdminDashboardService } from '../../../services/dashboardService';
import { OrdersByMonth, OrderStatusChart } from '../../../models/admin/admin-dashboard/kpi.model';
import { NgClass } from '@angular/common';
import { OrdersByMonthChartComponent } from '../../../shared-components/orders-by-month-chart-component/orders-by-month-chart-component';
import { OrderStatusTableComponent } from '../../../shared-components/order-status-table-component/order-status-table-component';

@Component({
  selector: 'app-admin-order-analytics',
  imports: [OrderStatusTableComponent,OrdersByMonthChartComponent],
  templateUrl: './admin-order-analytics.html',
  styleUrl: './admin-order-analytics.css',
})
export class AdminOrderAnalytics {
  private dashboardService = inject(AdminDashboardService);

  orderStatus = signal<OrderStatusChart[]>([]);
  ordersByMonth = signal<OrdersByMonth[]>([]);
  loadingOrderStatus = signal(false);
  loadingOrdersByMonth = signal(false);

  ngOnInit(): void {
    this.loadOrderStatus();
    this.loadOrdersByMonth();
  }

  private loadOrderStatus(): void {
    this.loadingOrderStatus.set(true);
    this.dashboardService.getOrdersByStatus().subscribe({
      next: res => {
        this.orderStatus.set(res);
        this.loadingOrderStatus.set(false);
      },
      error: err => {
        console.error(err);
        this.loadingOrderStatus.set(false);
      }
    });
  }
  private loadOrdersByMonth(): void {
    this.loadingOrdersByMonth.set(true);
    this.dashboardService.getOrdersByMonth().subscribe({
      next: res => {
        this.ordersByMonth.set(res);
        this.loadingOrdersByMonth.set(false);
      },
      error: err => {
        console.error(err);
        this.loadingOrdersByMonth.set(false);
      }
    });
  }
}

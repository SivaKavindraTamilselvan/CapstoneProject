import { Component, inject, signal } from '@angular/core';
import { VendorDashboardService } from '../../../services/vendor-dashBoard.Service';
import { OrdersByMonth, OrderStatusChart } from '../../../models/admin/admin-dashboard/kpi.model';
import { OrderStatusTableComponent } from '../../../shared-components/order-status-table-component/order-status-table-component';
import { OrdersByMonthChartComponent } from '../../../shared-components/orders-by-month-chart-component/orders-by-month-chart-component';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-vendor-order-analysis',
  imports: [OrderStatusTableComponent, OrdersByMonthChartComponent],
  templateUrl: './vendor-order-analysis.html',
  styleUrl: './vendor-order-analysis.css',
})
export class VendorOrderAnalysis {
  private dashboardService = inject(VendorDashboardService);

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
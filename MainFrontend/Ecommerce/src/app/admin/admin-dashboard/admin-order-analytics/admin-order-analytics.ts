import { Component, computed, inject, signal } from '@angular/core';
import { AdminDashboardService } from '../../../services/dashboardService';
import { OrdersByMonth, OrderStatusChart } from '../../../models/admin/admin-dashboard/kpi.model';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-order-analytics',
  imports: [NgClass],
  templateUrl: './admin-order-analytics.html',
  styleUrl: './admin-order-analytics.css',
})
export class AdminOrderAnalytics {
  private dashboardService = inject(AdminDashboardService);

  orderStatus = signal<OrderStatusChart[]>([]);
  ordersByMonth = signal<OrdersByMonth[]>([]);

  maxOrderCount = computed(() =>
    Math.max(...this.ordersByMonth().map(x => x.count), 1)
  );

  ngOnInit(): void {
    this.loadOrderStatus();
    this.loadOrdersByMonth();
  }

  private loadOrderStatus(): void {
    this.dashboardService.getOrdersByStatus().subscribe({
      next: res => this.orderStatus.set(res),
      error: err => console.error(err)
    });
  }

  private loadOrdersByMonth(): void {
    this.dashboardService.getOrdersByMonth().subscribe({
      next: res => this.ordersByMonth.set(res),
      error: err => console.error(err)
    });
  }
  monthBarWidth(count: number): number {
    const max = this.maxOrderCount();
    return max > 0 ? (count / max) * 100 : 0;
  }

  statusClasses(status: string): string {
    const key = status.toLowerCase();
    if (key.includes('cancel') || key.includes('reject')) return 'bg-rose-50 text-rose-700 ring-1 ring-rose-200';
    if (key.includes('pending') || key.includes('process')) return 'bg-amber-50 text-amber-700 ring-1 ring-amber-200';
    if (key.includes('deliver') || key.includes('complet')) return 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200';
    if (key.includes('ship')) return 'bg-sky-50 text-sky-700 ring-1 ring-sky-200';
    return 'bg-slate-50 text-slate-600 ring-1 ring-slate-200';
  }
}

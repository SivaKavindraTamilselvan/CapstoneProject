import { Component, inject, signal } from '@angular/core';
import { RevenueTrend } from '../../../models/admin/admin-dashboard/kpi.model';
import { AdminDashboardService } from '../../../services/dashboardService';
import { CurrencyPipe } from '@angular/common';
import { RevenueChartComponent } from '../../../shared-components/revenue-chart-component/revenue-chart-component';

@Component({
  selector: 'app-admin-revenue-chart',
  imports: [RevenueChartComponent],
  templateUrl: './admin-revenue-chart.html',
  styleUrl: './admin-revenue-chart.css',
})
export class AdminRevenueChart {
  private dashboardService = inject(AdminDashboardService);

  period = '7days';
  revenueTrend = signal<RevenueTrend[]>([]);

  ngOnInit(): void {
    this.loadRevenueTrend(this.period);
  }

  onPeriodChange(period: string) {
    this.period = period;
    this.loadRevenueTrend(period);
  }

  private loadRevenueTrend(period: string) {
    this.dashboardService.getRevenueTrend(period).subscribe({
      next: (res) => this.revenueTrend.set(res),
      error: () => this.revenueTrend.set([])
    });
  }
}

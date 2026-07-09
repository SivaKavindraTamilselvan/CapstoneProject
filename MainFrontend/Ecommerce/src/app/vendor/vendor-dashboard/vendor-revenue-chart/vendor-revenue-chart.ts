import { Component, inject, signal } from '@angular/core';
import { RevenueTrend } from '../../../models/admin/admin-dashboard/kpi.model';
import { VendorDashboardService } from '../../../services/vendor-dashBoard.Service';
import { CurrencyPipe } from '@angular/common';
import { RevenueChartComponent } from '../../../shared-components/revenue-chart-component/revenue-chart-component';

@Component({
  selector: 'app-vendor-revenue-chart',
  imports: [RevenueChartComponent],
  templateUrl: './vendor-revenue-chart.html',
  styleUrl: './vendor-revenue-chart.css',
})
export class VendorRevenueChart {
  private dashboardService = inject(VendorDashboardService);

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

import { Component, inject } from '@angular/core';
import { RevenueTrend } from '../../../models/admin/admin-dashboard/kpi.model';
import { AdminDashboardService } from '../../../services/dashboardService';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-admin-revenue-chart',
  imports: [CurrencyPipe],
  templateUrl: './admin-revenue-chart.html',
  styleUrl: './admin-revenue-chart.css',
})
export class AdminRevenueChart {
   private dashboardService = inject(AdminDashboardService);

  period = '7days';

  revenueTrend: RevenueTrend[] = [];

  ngOnInit(): void {
    this.loadRevenue();
  }

  loadRevenue(): void {
    this.dashboardService.getRevenueTrend(this.period).subscribe({
      next: res => {
        this.revenueTrend = res;
      }
    });
  }

  changePeriod(period: string): void {
    this.period = period;
    this.loadRevenue();
  }
}

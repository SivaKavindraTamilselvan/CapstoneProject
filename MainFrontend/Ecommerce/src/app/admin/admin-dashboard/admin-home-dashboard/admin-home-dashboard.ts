import { Component } from '@angular/core';
import { AdminKpiCard } from '../admin-kpi-card/admin-kpi-card';
import { AdminRevenueChart } from '../admin-revenue-chart/admin-revenue-chart';
import { AdminProductAnalytics } from '../admin-product-analytics/admin-product-analytics';
import { AdminOrderAnalytics } from '../admin-order-analytics/admin-order-analytics';

@Component({
  selector: 'app-admin-home-dashboard',
  imports: [AdminKpiCard,AdminRevenueChart,AdminProductAnalytics,AdminOrderAnalytics],
  templateUrl: './admin-home-dashboard.html',
  styleUrl: './admin-home-dashboard.css',
})
export class AdminHomeDashboard {

}

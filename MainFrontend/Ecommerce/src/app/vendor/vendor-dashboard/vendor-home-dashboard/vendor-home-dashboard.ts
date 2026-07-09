import { Component } from '@angular/core';
import { VendorRevenueChart } from '../vendor-revenue-chart/vendor-revenue-chart';
import { VendorProductAnalysis } from '../vendor-product-analysis/vendor-product-analysis';
import { VendorOrderAnalysis } from '../vendor-order-analysis/vendor-order-analysis';
import { VendorKpiCard } from '../vendor-kpi-card/vendor-kpi-card';

@Component({
  selector: 'app-vendor-home-dashboard',
  imports: [VendorRevenueChart,VendorProductAnalysis,VendorOrderAnalysis,VendorKpiCard],
  templateUrl: './vendor-home-dashboard.html',
  styleUrl: './vendor-home-dashboard.css',
})
export class VendorHomeDashboard {}

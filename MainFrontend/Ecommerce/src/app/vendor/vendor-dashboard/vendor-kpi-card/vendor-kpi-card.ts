import { Component, inject, signal } from '@angular/core';
import { VendorDashboardService } from '../../../services/vendor-dashBoard.Service';
import { VendorDashboardKPI } from '../../../models/admin/admin-dashboard/kpi.model';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-vendor-kpi-card',
  imports: [CurrencyPipe],
  templateUrl: './vendor-kpi-card.html',
  styleUrl: './vendor-kpi-card.css',
})
export class VendorKpiCard {
  private dashboardService = inject(VendorDashboardService);

  dashboard = signal( new VendorDashboardKPI());
  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.dashboardService.getDashboard().subscribe({
      next: (res) => {
        this.dashboard.set(res)
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}

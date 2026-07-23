import { Component, inject, signal } from '@angular/core';
import { AdminDashboardService } from '../../../services/dashboardService';
import { AdminDashboardKPI } from '../../../models/admin/admin-dashboard/kpi.model';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-admin-kpi-card',
  imports: [CurrencyPipe],
  templateUrl: './admin-kpi-card.html',
  styleUrl: './admin-kpi-card.css',
})
export class AdminKpiCard {
  private dashboardService = inject(AdminDashboardService);

  dashboard = signal( new AdminDashboardKPI());
  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.dashboardService.getDashboard().subscribe({
      next: (res) => {
        this.dashboard.set(res)
        console.log(res);
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}

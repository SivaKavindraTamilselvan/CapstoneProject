import { Component, inject, signal, computed } from '@angular/core';
import { AdminDashboardService } from '../../../services/dashboardService';
import { ProductApprovalStatus, ProductSubCategory } from '../../../models/admin/admin-dashboard/kpi.model';
import { NgClass } from '@angular/common';
import { ApprovalStatusComponenet } from '../../../shared-components/approval-status-componenet/approval-status-componenet';
import { SubcategoryBarChartComponent } from '../../../shared-components/subcategory-bar-chart-component/subcategory-bar-chart-component';

@Component({
  selector: 'app-admin-product-analytics',
  imports: [ApprovalStatusComponenet,SubcategoryBarChartComponent],
  templateUrl: './admin-product-analytics.html',
  styleUrl: './admin-product-analytics.css',
})
export class AdminProductAnalytics {
  private dashboardService = inject(AdminDashboardService);

  approvalStatus = signal<ProductApprovalStatus[]>([]);
  subCategories = signal<ProductSubCategory[]>([]);
  loadingApproval = signal(true);
  loadingSubCategories = signal(true);

  ngOnInit(): void {
    this.loadApprovalStatus();
    this.loadSubCategories();
  }

  loadApprovalStatus() {
    this.dashboardService.getProductApprovalStatus().subscribe({
      next: res => {
        this.approvalStatus.set(res);
        this.loadingApproval.set(false);
      },
      error: () => this.loadingApproval.set(false)
    });
  }

  loadSubCategories() {
    this.dashboardService.getProductsPerSubCategory().subscribe({
      next: res => {
        this.subCategories.set(res);
        this.loadingSubCategories.set(false);
      },
      error: () => this.loadingSubCategories.set(false)
    });
  }
}
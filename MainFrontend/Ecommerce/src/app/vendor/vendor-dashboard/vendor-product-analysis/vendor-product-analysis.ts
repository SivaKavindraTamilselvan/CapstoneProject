import { Component, signal } from '@angular/core';
import { SubcategoryBarChartComponent } from '../../../shared-components/subcategory-bar-chart-component/subcategory-bar-chart-component';
import { ApprovalStatusComponenet } from '../../../shared-components/approval-status-componenet/approval-status-componenet';
import { VendorDashboardService } from '../../../services/vendor-dashBoard.Service';
import { ProductApprovalStatus, ProductSubCategory } from '../../../models/admin/admin-dashboard/kpi.model';

@Component({
  selector: 'app-vendor-product-analysis',
  imports: [SubcategoryBarChartComponent, ApprovalStatusComponenet],
  templateUrl: './vendor-product-analysis.html',
  styleUrl: './vendor-product-analysis.css',
})
export class VendorProductAnalysis {
  constructor(private dashboardService: VendorDashboardService) { }
  ngOnInit() {
    this.loadApprovalStatus();
    this.loadSubCategories();
  }
  approvalStatus = signal<ProductApprovalStatus[]>([]);
  subCategories = signal<ProductSubCategory[]>([]);
  loadingApproval = signal(true);
  loadingSubCategories = signal(true);

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
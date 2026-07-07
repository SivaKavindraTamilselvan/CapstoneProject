import { Component, inject, signal, computed } from '@angular/core';
import { AdminDashboardService } from '../../../services/dashboardService';
import { ProductApprovalStatus, ProductSubCategory } from '../../../models/admin/admin-dashboard/kpi.model';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-admin-product-analytics',
  imports: [NgClass],
  templateUrl: './admin-product-analytics.html',
  styleUrl: './admin-product-analytics.css',
})
export class AdminProductAnalytics {
  private dashboardService = inject(AdminDashboardService);

  approvalStatus = signal<ProductApprovalStatus[]>([]);
  subCategories = signal<ProductSubCategory[]>([]);
  loadingApproval = signal(true);
  loadingSubCategories = signal(true);

  // Normalize bar widths against the largest count, not a fixed magic number
  maxSubCategoryCount = computed(() => {
    const items = this.subCategories();
    return items.length ? Math.max(...items.map(i => i.count)) : 0;
  });

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

  barWidth(count: number): number {
    const max = this.maxSubCategoryCount();
    return max > 0 ? (count / max) * 100 : 0;
  }

  statusClasses(status: string): string {
    const key = status.toLowerCase();
    if (key.includes('reject')) return 'bg-rose-50 text-rose-700 ring-1 ring-rose-200';
    if (key.includes('pending')) return 'bg-amber-50 text-amber-700 ring-1 ring-amber-200';
    if (key.includes('approve')) return 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200';
    if (key.includes('delete')) return 'bg-slate-100 text-slate-600 ring-1 ring-slate-200';
    return 'bg-slate-50 text-slate-600 ring-1 ring-slate-200';
  }
}
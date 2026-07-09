import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { BaseURL } from '../environment';
import { OrdersByMonth, OrderStatusChart, ProductApprovalStatus, ProductSubCategory, RevenueTrend, VendorDashboardKPI } from '../models/admin/admin-dashboard/kpi.model';

@Injectable({
  providedIn: 'root'
})
export class VendorDashboardService {

  private http = inject(HttpClient);

  getDashboard(): Observable<VendorDashboardKPI> {
    return this.http.get<VendorDashboardKPI>(
      `${BaseURL}/VendorDashboard`
    );
  }

  getRevenueTrend(period: string): Observable<RevenueTrend[]> {
    return this.http.get<RevenueTrend[]>(
      `${BaseURL}/VendorDashboard/revenue-trend?period=${period}`
    );
  }

  getProductApprovalStatus(): Observable<ProductApprovalStatus[]> {
    return this.http.get<ProductApprovalStatus[]>(
      `${BaseURL}/VendorDashboard/product-approval-status`
    );
  }

  getProductsPerSubCategory(): Observable<ProductSubCategory[]> {
    return this.http.get<ProductSubCategory[]>(
      `${BaseURL}/VendorDashboard/products-per-subcategory`
    );
  }

  getOrdersByStatus(): Observable<OrderStatusChart[]> {
    return this.http.get<OrderStatusChart[]>(
      `${BaseURL}/VendorDashboard/orders-by-status`
    );
  }

  getOrdersByMonth(): Observable<OrdersByMonth[]> {
    return this.http.get<OrdersByMonth[]>(
      `${BaseURL}/VendorDashboard/orders-by-month`
    );
  }
}
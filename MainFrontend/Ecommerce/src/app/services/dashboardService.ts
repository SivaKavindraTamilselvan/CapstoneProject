import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminDashboardKPI, OrdersByMonth, OrderStatusChart, ProductApprovalStatus, ProductSubCategory, RevenueTrend } from '../models/admin/admin-dashboard/kpi.model';
import { BaseURL } from '../environment';

@Injectable({
    providedIn: 'root'
})
export class AdminDashboardService {

    private http = inject(HttpClient);


    getDashboard(): Observable<AdminDashboardKPI> {
        return this.http.get<AdminDashboardKPI>(
            `${BaseURL}/Dashboard`
        );
    }
      getRevenueTrend(period: string): Observable<RevenueTrend[]> {
    return this.http.get<RevenueTrend[]>(
      `${BaseURL}/Dashboard/revenue-trend?period=${period}`
    );
  }
  getProductApprovalStatus() {
  return this.http.get<ProductApprovalStatus[]>(
    `${BaseURL}/Dashboard/product-approval-status`
  );
}

getProductsPerSubCategory() {
  return this.http.get<ProductSubCategory[]>(
    `${BaseURL}/Dashboard/products-per-subcategory`
  );
}
getOrdersByStatus() {
  return this.http.get<OrderStatusChart[]>(
    `${BaseURL}/Dashboard/orders-by-status`
  );
}

getOrdersByMonth() {
  return this.http.get<OrdersByMonth[]>(
    `${BaseURL}/Dashboard/orders-by-month`
  );
}
}